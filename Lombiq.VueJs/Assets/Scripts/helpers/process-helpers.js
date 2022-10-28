const { handleErrorMessage } = require('nodejs-extensions/scripts/handle-error');
const path = require('path');

const args = process.argv.splice(2);
const argumentOptions = args.length >= 2 ? JSON.parse(args[1]) : undefined;

async function executeFunctionByCommandLineArgument(functions) {
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
    const location = process.cwd().split(path.sep).slice(-2).join('/');
    // Need to check both because Windows and Linux resolve the directory symlink between the two differently.
    if (location === 'node_modules/lombiq-vuejs' || location === 'node_modules/.lv') {
        process.chdir(path.resolve('..', '..'));
    }
}

module.exports = {
    executeFunctionByCommandLineArgument,
    leaveNodeModule,
};
