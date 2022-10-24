# Lombiq Vue.js module for Orchard Core - Samples

## About

Example Orchard Core module that makes use of Lombiq Vue.js module for Orchard Core. For general details about and usage instructions see the [root Readme](../Readme.md).

Vue.js is a progressive framework for building user interfaces, mostly single-page applications. What if you want to build an SPA only for one feature or build multiple SPAs in one Orchard Core module? Here's an example of how to organize your Vue apps and components to keep them reusable and also have all the features we like in Orchard Core such as shape overrides and localization. Besides applications using separate Javascript and HTML files, it supports [Single File Components](https://vuejs.org/guide/scaling-up/sfc.html) too, which encapsulate the template and logic of a single reusable component into a single file.

If you are not familiar with Vue.js you can skip this tutorial or give it a try and see how cool of a framework it is. See: <https://vuejs.org/guide/introduction.html>

If you'd like to learn by example, check out our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## Training sections

You can start with either top level sections. Indented sections assume you've already read through their parents.  

- [Writing Vue.js applications in Orchard Core modules](Controllers/VueAppController.cs)
- [Writing Vue.js Single File Components in Orchard Core modules](Controllers/VueSfcController.cs)
  - [Progressive enhancement with SFCs](Controllers/VueSfcController.cs#L31)

## Miscellaneous Notes

You may have noticed that the _package.json_ contains `vue` even though it's not strictly necessary. It is there to activate the Vue plugin in Rider.

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
