const path = require('path');
const fs = require('fs');

module.exports = (rootPath) =>
    fs
        .readdirSync(rootPath)
        .filter(function (file) {
            return fs.statSync(path.join(rootPath, file)).isDirectory();
        });