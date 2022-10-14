const fs = require('fs');
const path = require('path');
const { minify } = require('terser');
const { rollup } = require('rollup');

function createDirectory(directoryPath) {
    return fs.existsSync(directoryPath) ? Promise.resolve() : fs.promises.mkdir(directoryPath);
}

// Used for setting defaults and preventing it would be needlessly complicated.
/* eslint-disable no-param-reassign */

module.exports = function rollupPipeline(
    destinationPath,
    filesAndEntryPaths,
    rollupPlugins,
    rollupOptions = null,
    outputFileNameTransform = null) {
    function configure(fileName, entryPath) {
        const defaultRollupOptions = {
            onwarn: (warning, next) => {
                if (warning.code === 'THIS_IS_UNDEFINED') return;
                next(warning);
            },
        };

        if (typeof rollupOptions === 'function') rollupOptions = rollupOptions(fileName, entryPath);
        if (typeof rollupPlugins === 'function') rollupPlugins = rollupPlugins(fileName, entryPath);

        const options = rollupOptions ? { ...defaultRollupOptions, ...rollupOptions } : defaultRollupOptions;
        options.input = entryPath;

        if (Array.isArray(rollupPlugins)) {
            if (Array.isArray(options.plugins)) {
                rollupPlugins.forEach((plugin) => options.plugins.push(plugin));
            }
            else {
                options.plugins = rollupPlugins;
            }
        }

        return options;
    }

    return Promise.all(filesAndEntryPaths
        .map(async (pair) => {
            const { fileName, entryPath } = pair;

            let bundle;
            try {
                const options = configure(fileName, entryPath);
                const outputOptions = { format: 'cjs' };

                bundle = await rollup(options);
                const { output } = await bundle.generate(outputOptions);

                await Promise.all(output.map(async (item) => {
                    if (item.type === 'asset') {
                        // This branch shouldn't ever be reached.
                        throw new Error(`Why is this an asset? (${JSON.stringify(item)})`);
                    }

                    const itemFileName = fs.existsSync(item.facadeModuleId) ? item.facadeModuleId : item.fileName;
                    const outputFileName = (typeof outputFileNameTransform === 'function')
                        ? outputFileNameTransform(itemFileName)
                        : itemFileName;
                    const outputPath = path.join(destinationPath, outputFileName + '.js');
                    await createDirectory(path.dirname(outputPath));
                    await fs.promises.writeFile(outputPath, item.code);

                    const minified = await minify(item.code, { sourceMap: true });
                    const minifiedPath = outputPath.replace(/\.js$/, '.min.js');
                    await fs.promises.writeFile(minifiedPath, minified.code);
                    await fs.promises.writeFile(minifiedPath + '.map', minified.map);
                }));
            }
            catch (error) {
                return error;
            }
            finally {
                if (bundle) await bundle.close();
            }

            return null;
        }))
        .then((array) => array.filter((item) => item !== null));
};
