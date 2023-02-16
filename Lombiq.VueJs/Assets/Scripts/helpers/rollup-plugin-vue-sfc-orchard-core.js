const path = require('path');
const sourceMap = require('source-map');
const readFile = require('fs').promises.readFile;
const { ESLint } = require('eslint');

const { formatter } = require('.nx/scripts/eslint-msbuild-formatter');

function onlyScript(source) {
    for (let i = 0; i < source.length; i++) {
        if (source[i] === '<') {
            let script = source.substring(i + 1).trim();

            if (script.startsWith('script')) {
                script = script.substring(script.indexOf('>') + 1);

                for (let j = script.length - 2; j >= 0; j--) {
                    if (!(script[j] !== '<' ||
                        script[j + 1] !== '/' ||
                        !script.substring(j + 2).trim().startsWith('script'))) {
                        let inner = script.substring(0, j);
                        while (inner.startsWith('\n')) inner = inner.substring(1);

                        return inner;
                    }
                }
            }
        }
    }

    throw new Error("Couldn't find the <script> block.");
}

function lastItem(array) {
    return array[array.length - 1];
}

async function lintScript(code, id, firstRow) {
    const eslint = new ESLint({ errorOnUnmatchedPattern: false });
    const results = await eslint.lintText(code, { filePath: id });

    if (!Array.isArray(results) || results.length === 0) return;

    for (let i = 0; i < results.length; i++) {
        const result = results[i];

        result.filePath = result.filePath.replace(/\.vue\?vue-sfc-entry$/, '.vue');

        for (let j = 0; j < result.messages.length; j++) {
            const message = result.messages[j];
            message.line += firstRow - 1;
        }
    }

    formatter(results);
}

module.exports = function vuePlugin() {
    return {
        name: 'rollup-plugin-vue-sfc-orchard-core',
        resolveId: async function (source, importer) {
            if (!source.toLowerCase().endsWith('.vue')) return null;

            const isRelativePath = source.startsWith('./') || source.startsWith('../');

            return (isRelativePath && importer)
                ? `${path.join(path.dirname(importer.replace(/\?vue-sfc/, '')), source)}?vue-sfc`
                : `${source}?vue-sfc-entry`;
        },
        load: async function (id) {
            const isEntryComponent = id.endsWith('?vue-sfc-entry');
            if (!isEntryComponent && !id.endsWith('?vue-sfc')) return null;

            const filePath = id.replace(/\?vue-sfc(-entry)?$/, '');

            // Get and trim the source code.
            const source = await readFile(filePath, 'utf8');
            let code = onlyScript(source).trim();

            // Reappend leading space. #spell-check-ignore-line
            const leadingSpace = lastItem(source.substring(0, source.indexOf(code) + 1).split('\n'));
            if (leadingSpace && leadingSpace.match(/^\s+$/)) code = leadingSpace + code;

            // Create mapping information and restore first line's indent.
            const first = source.substring(0, source.indexOf(code) + 1).split('\n');
            const firstRow = first.length;
            const firstRowColumnOffset = lastItem(first);
            for (let i = 1; i < firstRowColumnOffset; i++) code = ' ' + code;

            // Create mapping for the first row.
            const map = new sourceMap.SourceMapGenerator();
            map.addMapping({
                generated: {
                    line: 1,
                    column: 1,
                },
                source: filePath,
                original: {
                    line: firstRow,
                    column: 1,
                },
            });

            const filePathParts = filePath.split(/[\\/]/);
            const componentName = filePathParts[filePathParts.length - 1].replace(/\.vue$/i, '');
            const pascalCaseName = componentName
                .split('-')
                .map((word) => word[0].toUpperCase() + word.substring(1).toLowerCase())
                .join('');
            const className = 'VueComponent-' + pascalCaseName;

            // Inject name and template properties. The "(?<!\/\/.*)" is a negative lookbehind to ignore comments.
            const pattern = /(?<!\/\/.*)export\s+default\s*{/;
            if (!code.match(pattern)) throw new Error("Couldn't match 'export default {' in the source code!");
            code = code.replace(
                pattern,
                `export default { name: '${componentName}', template: document.querySelector('.${className}').innerHTML,` +
                // This line is intentionally compressed to simplify the mapping.
                ' /* eslint-disable-line */' +
                // We can't verify this rule, because line breaks added by the code will always be LF even on Windows
                // and it would be needless complexity. If you have linebreak issues they will crop up in your pure JS
                // files anyway.
                ' /* eslint-disable linebreak-style */');

            if (isEntryComponent) {
                code = code.replace(pattern, `window.Vue.component('${componentName}', {`);

                if (code[code.length - 1] === '}') {
                    // If the last character is the closing brace that needs to be treated separately. No semicolon here
                    // because we want ESLint to know about it not being there in the original source.
                    code += ')';
                }
                else {
                    let finalSemicolon = code.length - 1;
                    for (; finalSemicolon > 0 && code[finalSemicolon] !== ';'; finalSemicolon--) {
                        // Prevent overshooting if the optional semicolon is not used in the final expression.
                        if (finalSemicolon > 0 && code[finalSemicolon - 1] === '}') break;
                    }
                    code = code.substring(0, finalSemicolon) + ')' + (code[finalSemicolon] === ';' ? ';' : '');
                }
            }

            // Add trailing newline. This is not normal for .vue files but expected from .js files.
            code += '\n';

            // Run ESLint. We do it here instead of the Rollup plugin pipeline to limit analysis to the .vue file only.
            await lintScript(code, id, firstRow);

            return { code, map };
        },
    };
};
