const { readdirSync } = require('fs');

const asDirent = { withFileTypes: true };

function readdirentoryFunction(filter) {
    return (rootPath) => readdirSync(rootPath, asDirent).filter(filter).map((dirent) => dirent.name);
}

module.exports = {
    getVueApps: readdirentoryFunction((dirent) => dirent.isdirentory()),
    getVueComponents: readdirentoryFunction((dirent) => dirent.name.endsWith('.vue') && dirent.isFile()),
};
