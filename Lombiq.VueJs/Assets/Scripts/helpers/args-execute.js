const args = process.argv.splice(2);
const argumentOptions = args.length >= 2 ? JSON.parse(args[1]) : undefined;

module.exports = function argsExecute(functions) {
    if (!args[0]) return;

    const argument = args[0].toLowerCase();
    const target = Object
        .entries(functions)
        .filter((pair) => pair[0].toLowerCase() === argument)[0];

    if (target) target[1](argumentOptions);
};
