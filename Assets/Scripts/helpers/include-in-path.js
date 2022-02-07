// Automatically amend this project's node_modules to your node search path.
const path = require('path');

const isWindows = process.platform === 'win32';
const pathSeparator = isWindows ? ';' : ':';

const paths = process.env.NODE_PATH ? process.env.NODE_PATH.split(pathSeparator) : [];
paths.push(path.resolve(__dirname, '../../../node_modules'));

process.env.NODE_PATH = paths.join(pathSeparator);
require('module').Module._initPaths();
