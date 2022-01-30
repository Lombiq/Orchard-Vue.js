const path = require('path');
const sourceMap = require('source-map');
const readFile = require('fs').promises.readFile;
const log = require('fancy-log');

function onlyScript(source) {
    for (let i = 0; i < source.length; i++)
    {
        if (source[i] !== '<') continue;

        let script =    source.substr(i + 1).trim();
        if (!script.startsWith('script')) continue;

        script = script.substr(script.indexOf('>') + 1);
        for (let j = script.length - 2; j >= 0; j--)
        {
            if (script[j] !== '<' ||
                script[j + 1] !== '/' ||
                !script.substr(j + 2).trim().startsWith('script')) {
                continue;
            }

            let inner = script.substr(0, j);
            while (inner.startsWith('\n')) inner = inner.substr(1);

            return inner;
        }
    }
}

module.exports = function vuePlugin() {
    return {
        name: 'rollup-plugin-vue-sfc-orchard-core',
        async resolveId(source, importer) {
            if (!source.toLowerCase().endsWith('.vue')) return null;

            return (source.startsWith('./') && importer)
                ? `${path.join(path.dirname(importer.replace(/\?vue-sfc/, '')), source)}?vue-sfc`
                : `${source}?vue-sfc`;
        },
        async load (id) {
            if (!id.endsWith('?vue-sfc')) return null;

            const filePath = id.replace(/\?vue-sfc$/, '');

            // Get and trim the source code.
            const source = await readFile(filePath, 'utf8')
            let code = onlyScript(source);

            // Create mapping information and restore first line's indent.
            const first = source.substr(0, source.indexOf(code) + 1).split('\n');
            const firstRow = first.length;
            const firstRowColumnOffset = first[first.length - 1].length;
            for (let i = 1; i < firstRowColumnOffset; i++) code = ' ' +  code;

            // Create mapping for the first row.
            const map = new sourceMap.SourceMapGenerator();
            map.addMapping({
                generated: {
                    line: 1,
                    column: 1
                },
                source: filePath,
                original: {
                    line: firstRow,
                    column: 1
                }
            });

            const filePathParts = filePath.split(/[\\/]/);
            const componentName = filePathParts[filePathParts.length -1].replace(/\.vue$/i, '');
            const pascalCaseName = componentName
                .split('-')
                .map((word) => word[0].toUpperCase() + word.substr(1).toLowerCase())
                .join('');
            const className = 'VueComponent-' + pascalCaseName;

            // Inject name and template properties.
            const pattern = /export\s+default\s*{/;
            if (!code.match(pattern)) throw new Error("Couldn't match 'export default {' in the source code!");
            code = code.replace(pattern, `export default { name: '${componentName}', template: '.${className}',`);

            return { code, map };
        },
    };
}
