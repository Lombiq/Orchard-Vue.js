const getCwd = require('.nx/scripts/get-cwd');
const { handleErrorObject, handleErrorMessage } = require('.nx/scripts/handle-error');
const path = require('path');

async function executeFunctionByCommandLineArgument(functions) {
    const [functionName, argumentOptionsJson] = process.argv.slice(2);
    const argumentOptions = argumentOptionsJson ? JSON.parse(argumentOptionsJson) : undefined;

    // Informs the consumer of correct usage and exits if something is missing.
    function helpAndExit(message) {
        handleErrorMessage(message + ' (USAGE: node <script-file> <compile|clean> [options-json])');
        process.exit(1);
    }

    if (!functionName) helpAndExit('Command line argument is expected.');

    try {
        // Look for the target function by property key (case-insensitive).
        const target = Object
            .entries(functions)
            .filter(([key]) => key.toLowerCase() === functionName)[0];

        if (!target) helpAndExit(`Couldn't find the function "${functionName}".`);

        // #spell-check-ignore-line: Execute the function with the provided options object, if any. If it's a thenable
        // object (e.g. Promise), then it's also awaited.
        const task = target[1](argumentOptions);
        if (task && task.then) await task;
    }
    catch (error) {
        handleErrorObject(error);
        process.exit(1);
    }
}

function leaveNodeModule() {
    const cwd = getCwd();
    const location = cwd.split(path.sep).slice(-2).join('/');

    if (location === 'node_modules/lombiq-vuejs') {
        process.chdir(path.join(cwd, '..', '..'));
    }
}

module.exports = {
    executeFunctionByCommandLineArgument,
    leaveNodeModule,
};
