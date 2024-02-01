const fs = require('fs');

function tryOpenAndParse(path) {
    let text;

    try { text = fs.readFileSync(path); }
    catch { return { }; }

    return JSON.parse(text);
}

module.exports = tryOpenAndParse;