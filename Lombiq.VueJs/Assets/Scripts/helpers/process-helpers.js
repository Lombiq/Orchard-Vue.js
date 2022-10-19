const { handleErrorMessage } = require('nodejs-extensions/scripts/handle-error');
const path = require('path');

const args = process.argv.splice(2);
const argumentOptions = args.length >= 2 ? JSON.parse(args[1]) : undefined;

async function argsExecute(functions) {
    if (!args[0]) return;

    try {
        const argument = args[0].toLowerCase();
        const target = Object
            .entries(functions)
            .filter((pair) => pair[0].toLowerCase() === argument)[0];

        if (target) {
            const task = target[1](argumentOptions);
            if (task && task.then) await task;
        }
    }
    catch (error) {
        handleErrorMessage(error);
        process.exit(1);
    }
}

function leaveNodeModule() {
    if (process.cwd().split(path.sep).slice(-2).join('/') === 'node_modules/lombiq-vuejs') {
        process.chdir(path.resolve('..', '..'));
    }
}

module.exports = {
    argsExecute,
    leaveNodeModule,
}
