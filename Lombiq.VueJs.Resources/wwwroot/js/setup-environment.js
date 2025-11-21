// Ensure that variables required by vue-router are initialized. Without these it will crash out while trying to load
// the dev tools. Those dev tools can't be used from the browser anyway, they are only for node.js developers.

// eslint-disable-next-line no-underscore-dangle -- We have no control over the name of this variable.
if (window.__VUE_PROD_DEVTOOLS__ === undefined) window.__VUE_PROD_DEVTOOLS__ = false;
if (typeof window.process !== 'object') window.process = {};
if (typeof window.process.env !== 'object') window.process.env = {};
if (!window.process.env.NODE_ENV?.trim()) window.process.env.NODE_ENV = 'production';
