# Lombiq Vue.js module for Orchard Core

[![Lombiq.VueJs NuGet](https://img.shields.io/nuget/v/Lombiq.VueJs?label=Lombiq.VueJs)](https://www.nuget.org/packages/Lombiq.VueJs/) [![Lombiq.VueJs.Samples NuGet](https://img.shields.io/nuget/v/Lombiq.VueJs.Samples?label=Lombiq.VueJs.Samples)](https://www.nuget.org/packages/Lombiq.VueJs.Samples/) [![Lombiq.VueJs.Tests.UI NuGet](https://img.shields.io/nuget/v/Lombiq.VueJs.Tests.UI?label=Lombiq.VueJs.Tests.UI)](https://www.nuget.org/packages/Lombiq.VueJs.Tests.UI/)

## About

[Orchard Core](http://orchardproject.net/) module that contains [Vue.js](https://vuejs.org/) and commonly used Vue.js components to be used in other Vue.js apps as dependencies. Provides extensibility to create Vue.js component templates as Orchard Core shapes making them able to override in themes or modules.

We at [Lombiq](https://lombiq.com/) also used this module for the following projects:

- The multi-tenant church community management system [Kast](https://www.kast.io/) ([see case study](https://lombiq.com/blog/helping-kast-build-a-multi-tenant-platform-on-orchard-core)).
- The new [Lombiq website](https://lombiq.com/) when migrating it from Orchard 1 to Orchard Core ([see case study](https://lombiq.com/blog/how-we-renewed-and-migrated-lombiq-com-from-orchard-1-to-orchard-core)).
- The new client portal for [WTW](https://www.wtwco.com/) ([see case study](https://lombiq.com/blog/lombiq-s-journey-with-wtw-s-client-portal)).

If you just want to see the whole thing in action, check out the [Samples project](Lombiq.VueJs.Samples/Readme.md). Do you want to quickly try it out? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

> ⚠️ Starting version 4.0, this Orchard Core module is using Vue 3 and only supports SFC compilation. Resources are now imported as [Javascript modules](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Modules), so the Vue 3 used by your component is independent from Orchard Core's built-in `vuejs` [resource](https://docs.orchardcore.net/en/main/docs/resources/libraries/) and the two can exist on the same page. If you have an existing project that relies on a discontinued version of Vue.js, check out [our migration instructions below](#migrating-from-vue-2).

## Prerequisites

1. This project relies on [Lombiq Node.js Extensions](https://github.com/Lombiq/NodeJs-Extensions/), please see its prerequisites [here](https://github.com/Lombiq/NodeJs-Extensions/#prerequisites).
2. If you are importing this project as a submodule, include the following entries at the beginning and end of your csproj file. If you are using the NuGet package, this is not necessary:

    ```xml
    <Project Sdk="Microsoft.NET.Sdk.Razor">
      <Import Project="..\..\..\Utilities\Lombiq.NodeJs.Extensions\Lombiq.NodeJs.Extensions\Lombiq.NodeJs.Extensions.props" />
      <Import Project="..\Lombiq.VueJs\Lombiq.VueJs.props" />
        
      <!-- Everything else goes here. -->
    
      <Import Project="..\..\..\Utilities\Lombiq.NodeJs.Extensions\Lombiq.NodeJs.Extensions\Lombiq.NodeJs.Extensions.targets" />
      <Import Project="..\Lombiq.VueJs\Lombiq.VueJs.targets" />
    </Project>
    ```

3. Create a _package.json_ file with the following content:

    ```json
    {
      "private": true,
      "scripts": {
        "build": "npm explore nodejs-extensions -- pnpm build && npm explore lombiq-vuejs -- pnpm build",
        "compile": "npm explore nodejs-extensions -- pnpm compile && npm explore lombiq-vuejs -- pnpm compile",
        "clean": "npm explore nodejs-extensions -- pnpm clean && npm explore lombiq-vuejs -- pnpm clean",
        "watch": "npm explore nodejs-extensions -- pnpm watch && npm explore lombiq-vuejs -- pnpm watch"
      }
    }
    ```

The packages will be automatically installed on build (i.e. `dotnet build`). You can also trigger the installation manually using the `pnpm install` command. Note that since dependencies are delivered as Javascript modules, you don't have to install the `vue` package in your consuming project as well.

### Configuration

The build script can be configured by placing a JSON file (called _vue-sfc-compiler-pipeline.json_) in the project root. It can contain the same properties you can see in the `defaultOptions` variables in [vue-sfc-compiler-pipeline.js](Lombiq.VueJs/Assets/scripts/helpers/vue-sfc-compiler-pipeline.js). Any property set in the JSON file overrides the default value as the two objects are merged.

When configuring the `rollupNodeResolve` option (for [`@rollup/plugin-node-resolve`](https://www.npmjs.com/package/@rollup/plugin-node-resolve)), normally you could only pass in an array of exact matches due to the limitation of the JSON format. Instead, you can use `rollupNodeResolve.resolveOnlyRules` which is an object array in the following format:

```json
{
  "rollupNodeResolve": {
    "preferBuiltins": true,
    "browser": true,
    "mainFields": ["module", "jsnext:main"],
    "resolveOnlyRules": [
      {
        "regex": false,
        "include": false,
        "value": "vue"
      },
      {
        "regex": true,
        " ": false,
        "value": "^vuetify"
      }
    ]
  },
  "isProduction": false
}
```

Here we excluded `vue` and packages starting with `vuetify` (e.g. `vuetify/components`) from the resolution, so they are treated as external. Then you can add `vuetify` using the resource manifest as a script module.

## Using Vue.js Single File Components

The module identifies Single File Components in the _Assets/Scripts/VueComponents_ directory and harvests them as shapes. They have a custom _.vue_ file renderer that displays the content of the `<template>` element after applying localization for the custom `[[ ... ]]` expression that calls `IStringLocalizer`. Besides that, it's pure Vue, yet you can still make use of shape overriding if needed.

See a demo video of using Vue.js Single File Components [here](https://www.youtube.com/watch?v=L0qjpQ6THZU).

### Writing SFCs

What you need to know to write your own _.vue_ file:

- Your component's script should have a `<template>` and `<script>` element in that order.
- The script must export the module as an object literal ESM style (`export default { ... }`).
  - You shouldn't specify the `name` and `template` properties, as these are automatically provided during compilation.
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

### Using SFCs

In most cases you'll want to place a full app on a page and initialize it with server-side data. You can achieve this by using the tag helper like this:

```razor
<vue-component-app area="My.Module" name="my-article" model="@new { Title = "Hello World!", Date = DateTime.Now }" plugins="resourceName1, resourceName2" />
```

This will automatically create a new Vue app that only contains the component identified by the `name` attribute (i.e. _/Assets/Scripts/VueComponents/my-article.vue_). The app gets the data from the `model` attribute which is bound to the component. (By the way if your .vue and .cshtml files are in the same Orchard Core module, then you can even skip the `area` attribute.) The only other consideration is that if your SFC has child components, those have to be declared in the Orchard Core resource manifest options. It would be like `_manifest.DefineSingleFileComponent("my-article").SetDependencies("my-child-component");`. Components that don't have children don't have to be declared this way.

The optional `plugins` attribute can contain a comma separated list of resource names. These are `script-module` resources that you can register like `_manifest.DefineScriptModule("resourceName1").SetUrl(...)`. Such modules are expected to `export default` a plugin object that can be dynamically imported and passed to `app.use()` (i.e. `app.use(await import('resourceName1').default)`).

For more details and a demo of the full feature set check out the samples project!

### Limitations and Considerations

- No other Razor features including string localizer with arguments.
- Including a script element in your template will break it. Although you shouldn't do that anyway.
- As you might expect from Orchard Core, the style element isn't supported either since you will be using themes. If you can think of a use-case that's applicable for OC, please open an issue.
- Our compiler extracts the `<script>` block from the SFC into a JS module instead of using the official `@vue/compiler-sfc`. The rationale there is that we don't want to compile the template into the script anyway so using the official compiler has limited benefits and significant added complexity over simply extracting the script from the blocks as plain text. The only drawback is that SFC-specific syntax (e.g. `<script setup>`) won't work.

Regarding the first three points: If you need anything more complicated, first reconsider your application design to see if your goals can be achieved in a more Vue.js logic. For example, pass the variables in your main app that hands them down via property binding. If you are certain you need more server side features, either use a _cshtml_ templated app as outlined above or use shape overriding on the `VueComponent-{FileNameInPascalCase}` shape.

## Migrating from Vue 2

Most importantly, if you are not familiar with the breaking changes in Vue 3, please read the [official migration guide](https://v3-migration.vuejs.org/).

Other changes:

- The `Vue.applications` object is no longer available. A similar `window.VueApplications` is created, with a more straight-forward structure that's not grouped by component type, but directly accessible by name (`window.VueApplications[app.$appId] = app`).
- The model passed to `<vue-component-app>` is now stored in the app's `viewModel` property. This means to access the view-model from JS you have to type `app.viewModel.propertyName` instead of `app.propertyName` as before.
- The element where the app is mounted with `<vue-component-app>` is now stored in the app's `root` property. This should be used instead of `app.$el`.
- If you are using the `vue-pagination` component provided by Lombiq.VueJs, the `page` component property has been renamed to `modelValue`. If you were using it with `v-model` then no changes are necessary.

Also if your app still uses the `<vue-component>` tag helper directly, consider switching to `<vue-component-app>` to reduce future maintenance on your end.

## Dependencies

This module has the following dependencies:

- [Lombiq Helpful Libraries for Orchard Core](https://github.com/Lombiq/Helpful-Libraries)
- [Lombiq Node.js Extensions](https://gihub.com/Lombiq/NodeJs-Extensions)

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
