import fs from 'fs';

export function tryOpenAndParse(path) {
    let text;

    try { text = fs.readFileSync(path); }
    catch { return { }; }

    return JSON.parse(text);
}
