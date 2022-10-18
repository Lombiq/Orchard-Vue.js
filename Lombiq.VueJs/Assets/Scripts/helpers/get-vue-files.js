const { readdirSync } = require('fs');

const asDirect = { withFileTypes: true };

function readDirectoryFunction(filter) {
    return (rootPath) => readdirSync(rootPath, asDirect).filter(filter).map((direct) => direct.name);
}

module.exports = {
    getVueApps: readDirectoryFunction((direct) => direct.isDirectory()),
    getVueComponents: readDirectoryFunction((direct) => direct.name.endsWith('.vue') && direct.isFile()),
};
