# Lombiq Vue.js module for Orchard Core

[![Lombiq.VueJs NuGet](https://img.shields.io/nuget/v/Lombiq.VueJs?label=Lombiq.VueJs)](https://www.nuget.org/packages/Lombiq.VueJs/) [![Lombiq.VueJs.Samples NuGet](https://img.shields.io/nuget/v/Lombiq.VueJs.Samples?label=Lombiq.VueJs.Samples)](https://www.nuget.org/packages/Lombiq.VueJs.Samples/) [![Lombiq.VueJs.Tests.UI NuGet](https://img.shields.io/nuget/v/Lombiq.VueJs.Tests.UI?label=Lombiq.VueJs.Tests.UI)](https://www.nuget.org/packages/Lombiq.VueJs.Tests.UI/)

## About

[Orchard Core](http://orchardproject.net/) module that contains [Vue.js](https://vuejs.org/) and commonly used Vue.js components to be used in other Vue.js apps as dependencies. Provides extensibility to create Vue.js component templates as Orchard Core shapes making them able to override in themes or modules.

We at [Lombiq](https://lombiq.com/) also used this module for the following projects:

- The multi-tenant church community management system [Kast](https://www.kast.io/) ([see case study](https://lombiq.com/blog/helping-kast-build-a-multi-tenant-platform-on-orchard-core)).
- The new [Lombiq website](https://lombiq.com/) when migrating it from Orchard 1 to Orchard Core ([see case study](https://lombiq.com/blog/how-we-renewed-and-migrated-lombiq-com-from-orchard-1-to-orchard-core)).

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

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

## Sample project

If you just want to see the whole thing in action, check out the [Samples project](Lombiq.VueJs.Samples/Readme.md).

## Using Vue.js node packages

The packages will be automatically installed on build (i.e. `dotnet build`). You can also trigger the installation manually using the `pnpm install` command.

## Adding Vue.js component templates

Place your template files (.cshtml or .liquid) in the _Views/VueComponents_ folder. The shape template harvester will harvest these templates, and the generated shape type will be as it would be normally generated, but with a `VueComponent-` prefix. Eg.:

```cshtml
    <shape type="VueComponent-App_UserProfile"></shape>
```

In these shapes you can use any format you want (e.g. JSX templates) and reference their id in your Vue.js component's JavaScript code.

## Using Vue.js Single File Components

The module identifies Single File Components in the _Assets/Scripts/VueComponents_ directory and harvests them as shapes. They have a custom _.vue_ file renderer that displays the content of the `<template>` element after applying localization for the custom `[[ ... ]]` expression that calls `IStringLocalizer`. Besides that, it's pure Vue, yet you can still make use of shape overriding if needed.

See a demo video of using Vue.js Single File Components [here](https://www.youtube.com/watch?v=L0qjpQ6THZU).

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

You can include the `<vue-component area="My.Module" name="my-article">` tag helper in your code. This will add Vue and _My.Module/wwwroot/vue/my-article.js_ to the resource manager (using the `vue-component-{name}` resource) as well as the `VueComponent-MyArticle` shape (the SFC's kebab-case name is converted into PascalCase). Include `depends-on="vue-component-my-article"` in your app's `<script>` element to ensure correct load order.

The Rollup plugin automatically registers each component you include (but not their children) as globally accessible components. So you don't need to list them in your app's `components` property. Indeed the component object isn't exposed as a global variable.

If your Vue app is just going to include one top level component and bind to that, feel free to use this tag helper: `<vue-component-app area="My.Module" name="my-article" model="@data" model-property="value" id="unique-id" class="additional classes">`. Here area and name are the same as above, model will be converted into JSON using camelCase property names. The model-property, id and class are optional. If you don't specify a model-property it defaults to "value". If you have set custom `model.prop` in your component, set this to the same value to indicate that the property is to be set up with two-way binding via `v-model`. If you don't specify an id, `{componentName}_{Guid.NewGuid():N}` is used to guarantee uniqueness. An `$appId` property is appended to your data object which contains the id. If you need to work with this Vue object you can access it like this:

```html
<script at="Foot" depends-on="unique-id-VueApp">
    const app = Vue.applications['my-article'].filter((app) => app.$appId === 'unique-id')[0];
</script>
```

### Advantages of SFCs

- Tooling! If you have an IDE plugin for Vue.js it will work better. Syntax highlighting, property autocomplete, Go to Definition for custom elements, and all other advantages of static Vue development.
- The script and template are kept together, which makes understanding the individual component easier.
- No need to escape the `@` on events.

### Limitations and Considerations

- No other Razor features including string localizer with arguments.
- Including a script element in your template will break it. Although you shouldn't do that anyway.
- As you might expect from Orchard Core, the style element isn't supported either since you will be using themes. If you can think of a use-case that's applicable for OC, please open an issue.

Regarding the points: If you need anything more complicated, first reconsider your application design to see if your goals can be achieved in a more Vue.js logic. For example, pass the variables in your main app that hands them down via property binding. If you still need something else, either use a _cshtml_ templated app as outlined above or use shape overriding on the `VueComponent-{FileNameInPascalCase}` shape.

## Dependencies

This module has the following dependencies:

- [Lombiq Helpful Libraries for Orchard Core](https://github.com/Lombiq/Helpful-Libraries)
- [Lombiq Node.js Extensions](https://gihub.com/Lombiq/NodeJs-Extensions)

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
