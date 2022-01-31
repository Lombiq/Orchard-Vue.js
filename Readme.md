# Lombiq Vue.js module for Orchard Core



## About

[Orchard Core](http://orchardproject.net/) module that contains [Vue.js](https://vuejs.org/) and commonly used Vue.js components to be used in other Vue.js apps as dependencies. Provides extensibility to create Vue.js component templates as Orchard Core shapes making them able to override in themes or modules.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!


## Prerequisites

1. Make sure that you have the latest **14.x** version (it can't be a later one because Gulp [doesn't officially support 14.x even](https://github.com/gulpjs/gulp/discussions/2649), let alone more recent ones) of [Node.js](https://nodejs.org/en/) installed that fits your system architecture (x64 or x86).
2. Install or update NPM to the latest version using the command: `npm install --global npm@7.9.0`. You may also install [PNPM](https://pnpm.io/) with `npm install --global pnpm`.
3. Install or update the Gulp CLI globally with this command: `npm install -g gulp-cli`.
3. If you're using Visual Studio, then under ["External Web Tools"](https://devblogs.microsoft.com/dotnet/customize-external-web-tools-in-visual-studio-2015/) add the installation path of Node.js (most possibly *C:\Program Files\NodeJS**) to the list and move it to the top.


## Using Vue.js node packages

The packages will be automatically installed on build (i.e. `dotnet build`) or you can trigger it using the `npm install` command.


## Adding Vue.js component templates

Place your template files (.cshtml or .liquid) to the *Views/VueComponents* folder. The shape template harvester will harvest these templates and the generated shape type will be as it would be normally generated but with `VueComponent-` prefix. Eg.:

    <shape type="VueComponent-App_UserProfile"></shape>

In these shapes you can use any format you want (e.g. JSX templates) and reference their id in your Vue.js component JavaScript code.


## Using Vue.js Single File Components

The module identifies Single File Components in the _Assets/Scripts/VueComponents_ directory and harvests them as shapes. They have a custom _.vue_ file renderer that displays the content of the `<template>` element after applying localization for the custom `[[ ... ]]` expression that calls `IStringLocalizer`. Besides that it's pure Vue, yet you can still make use of shape overriding if needed.

What you need to know to write your own _.vue_ file:
- Your component's script should have a `<template>` and `<script>` element in that order.
- The script must export the module as an object literal ESM style (`export default { ... }`).
  - You don't need to specify the `name` and `template` properties, as these are automatically provided during compilation.
- If your component has child components, include the _.vue_ extension when importing them. 

For example if you have the file _My.Module/Assets/Scripts/VueComponents/my-article.vue_:
```vue
<template>
    <article>
        <header>
            <h3>{{ title }}</h3>
        </header>
        <slot></slot>
        <footer>{{ formattedDate }}</footer>
    </article>
</template>

<script>
export default {
    props: [ 'title', 'date' ],
    computed: {
        formattedDate(self) {
            const formatter = new Intl.DateTimeFormat(
                self.culture,
                {
                    year: 'numeric',
                    month: 'short',
                    day: 'numeric',
                });

            return formatter.format(self.date);
        }
    }
};
</script>
```

You can include the `<vue-component name="my-article">` tag helper in your code. This will add Vue and _My.Module/wwwroot/vue/my-article.js_ to the resource manager as well as the `VueComponent-MyArticle` shape (the SFC's kebab-case name is converted into PascalCase).


### Advantages of SFCs.

- The script and template are kept together, which makes understanding the individual component easier.
- If you have an IDE plugin or similar tooling for Vue.js it will work better.
- No need for `@@` on events.


### Limitations and Considerations

- No other Razor features including string localizer with arguments.
- Including a script element in your template will break it. Although you shouldn't do that anyway.
- As you might expect from Orchard Core, the style element isn't supported either since you will be using themes. If you can think of a use-case that's applicable for OC, please open an issue. 

Regarding the points: if you need anything more complicated, first reconsider you application design to see if your goals can be achieved in a more Vue.js logic. For example pass the variables in your main app that hands them down via property binding. If you still need something else, either use a _cshtml_ templated app as outlined above or use shape overriding on the `VueComponent-{FileNameInPascalCase}` shape.


## Other resources

Some resources are registered in the resource manifest so you can add these as dependencies to your Vue.js app's resource. These resources are automatically copied from the *node_modules* folder to *wwwroot* using Gulp when building the project (or you can trigger it with the `gulp` command).

- [ES6-Promise](https://www.npmjs.com/package/es6-promise): Use this if you want to use ES6 Promises (or an ES6 module that uses them e.g. `axios`) include this resource so it will work in IE as well.


## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
