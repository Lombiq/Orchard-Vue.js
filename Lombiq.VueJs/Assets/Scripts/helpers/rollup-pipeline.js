const fs = require('fs');
const path = require('path');
const { minify } = require('terser');
const { rollup } = require('rollup');

const { handleErrorObject, handlePromiseRejectionAsError } = require('.nx/scripts/handle-error');

function createDirectory(directoryPath) {
    return fs.existsSync(directoryPath) ? Promise.resolve() : fs.promises.mkdir(directoryPath, { recursive: true });
}

function handleRollupError(error) {
    const stack = error?.stack?.split(/[\n\r]+/).map((line) => line.trim()) ?? [];
    const errorLineRegex = /\.(js|vue):(?<line>\d+)(:(?<column>\d+))?.*/;
    const lineWithFilePath = stack
        .filter((line) => line.match(errorLineRegex))
        .map((line) => line.replace(/^\s*at\s+/, '').trim())[0];
    const lineMatch = lineWithFilePath?.match(errorLineRegex)?.groups ?? {};

    handleErrorObject({
        code: 'ROLLUP',
        path: lineWithFilePath,
        line: lineMatch.line,
        column: lineMatch.column,
        message: error,
    });
}

module.exports = function rollupPipeline(
    destinationPath,
    filesAndEntryPaths,
    rollupPlugins,
    rollupOptions = null,
    outputFileNameTransform = null,
    panicOnFailure = true) {
    function configure(fileName, entryPath) {
        const defaultRollupOptions = {
            onwarn: (warning, next) => { // #spell-check-ignore-line
                if (warning.code === 'THIS_IS_UNDEFINED') return;
                next(warning);
            },
        };

        let customRollupPlugins = rollupPlugins;
        let customRollupOptions = rollupOptions;

        if (typeof customRollupPlugins === 'function') customRollupPlugins = customRollupPlugins(fileName, entryPath);
        if (typeof customRollupOptions === 'function') customRollupOptions = customRollupOptions(fileName, entryPath);

        const options = customRollupOptions ? { ...defaultRollupOptions, ...customRollupOptions } : defaultRollupOptions;
        options.input = entryPath;

        // Safely copy the provided plugins to the options.
        if (Array.isArray(customRollupPlugins)) {
            if (Array.isArray(options.plugins)) {
                customRollupPlugins.forEach((plugin) => options.plugins.push(plugin));
            }
            else {
                options.plugins = customRollupPlugins;
            }
        }

        return options;
    }

    const pipelinePromise = Promise.all(filesAndEntryPaths
        .map(async ({ fileName, entryPath }) => {
            let success = true;
            let bundle;

            try {
                const options = configure(fileName, entryPath);
                const outputOptions = { format: 'cjs' }; // #spell-check-ignore-line

                bundle = await rollup(options);
                const { output } = await bundle.generate(outputOptions);

                await Promise.all(output.map(async (item) => {
                    try {
                        if (item.type === 'asset') {
                            // This branch shouldn't ever be reached.
                            throw new Error(`Why is this an asset? (${JSON.stringify(item)})`);
                        }

                        const itemFileName = fs.existsSync(item.facadeModuleId) ? item.facadeModuleId : item.fileName;
                        const outputFileName = typeof outputFileNameTransform === 'function'
                            ? outputFileNameTransform(itemFileName)
                            : itemFileName;
                        const outputPath = path.join(destinationPath, outputFileName + '.js');
                        await createDirectory(path.dirname(outputPath));
                        await fs.promises.writeFile(outputPath, item.code);

                        const minified = await minify(item.code, { sourceMap: true });
                        const minifiedPath = outputPath.replace(/\.js$/, '.min.js');
                        await fs.promises.writeFile(minifiedPath, minified.code);
                        await fs.promises.writeFile(minifiedPath + '.map', minified.map);
                    }
                    catch (error) {
                        try { handleRollupError(error); }
                        catch (innerError) { handleErrorObject(innerError); }
                        success = false;
                    }
                }));
            }
            finally {
                if (bundle) await bundle.close();
            }

            if (!success) throw new Error('rollupPipeline failed!');
        }));

    return handlePromiseRejectionAsError(pipelinePromise, panicOnFailure);
};
