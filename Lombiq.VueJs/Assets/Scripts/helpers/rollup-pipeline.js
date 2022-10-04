const path = require('path');
const rollup = require('rollup');
const { minify } = require("terser");

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
                console.log(bundle.watchFiles);

                const { output } = await bundle.generate(outputOptions);
                for (const item in output) {
                    if (item.type === 'asset') {
                        throw new Error(`Why is this an asset? (${ JSON.stringify(item) })`)
                    }

                    const outputFileName = (typeof outputFileNameTransform === 'function')
                        ? outputFileNameTransform(item.fileName)
                        : item.fileName;
                    const outputPath = path.join(destinationPath, outputFileName + '.js');
                    console.log(outputPath);
                    console.log(JSON.stringify(item));

                    const minifyResult = await minify(item.code, { sourceMap: true });
                    const minified = { ...item, code: minifyResult.code, map: minifyResult.map };
                    const minifiedPath = outputPath.replace(/\.js$/, '.min.js')
                    console.log(minifiedPath);
                    console.log(JSON.stringify(minified));
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
