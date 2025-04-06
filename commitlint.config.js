module.exports = {
    extends: ['@commitlint/config-conventional'],
    'rules': {
    'scope-enum': [2, 'always', [
        'app',
        'views',
        'viewmodels',
        'models',
        'services',
        'resources',
        'ci',
        'config'
    ]],
    },
};