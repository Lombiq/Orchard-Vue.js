const path = require('path');
const { minify } = require("terser");
const { rollup } = require('rollup');

function createDirectory(path) {
    return fs.existsSync(path) ? Promise.resolve() : fs.promises.mkdir(path);
}

module.exports = function rollupPipeline(
    destinationPath,
    filesAndEntryPaths,
    rollupPlugins,
    rollupOptions = null,
    outputFileNameTransform = null) {

    function configure(entryPath) {
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
                rollupPlugins.forEach(plugin => options.plugins.push(plugin));
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
                const options = configure(entryPath);
                const outputOptions = { format: 'cjs' };

                bundle = await rollup(options);
                const { output } = await bundle.generate(outputOptions);

                for (const item of output) {
                    if (item.type === 'asset') {
                        throw new Error(`Why is this an asset? (${ JSON.stringify(item) })`);
                    }

                    const outputFileName = (typeof outputFileNameTransform === 'function')
                        ? outputFileNameTransform(item.fileName)
                        : item.fileName;
                    const outputPath = path.join(destinationPath, outputFileName + '.js');
                    await createDirectory(path.dirname(outputPath));
                    await fs.promises.writeFile(outputPath, item.code);

                    const minified = await minify(item.code, { sourceMap: true });
                    const minifiedPath = outputPath.replace(/\.js$/, '.min.js')
                    await fs.promises.writeFile(minifiedPath, minified.code);
                    await fs.promises.writeFile(minifiedPath + '.map', minified.map);
                }
            }
            catch (error) {
                return error;
            }
            finally {
                if (bundle) await bundle.close();
            }
        }))
        .then((array) => array.filter(item => item !== undefined));
};
