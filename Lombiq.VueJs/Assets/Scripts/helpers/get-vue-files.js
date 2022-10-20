const { readdirSync } = require('fs');

const asDirent = { withFileTypes: true };

function readDirectoryFunction(filter) {
    return (rootPath) => readdirSync(rootPath, asDirent).filter(filter).map((dirent) => dirent.name);
}

module.exports = {
    getVueApps: readDirectoryFunction((dirent) => dirent.isDirectory()),
    getVueComponents: readDirectoryFunction((dirent) => dirent.name.endsWith('.vue') && dirent.isFile()),
};
