import xoTypeScript from 'eslint-config-xo-typescript';

// https://github.com/xojs/xo/issues/798
const xoTypeScriptPatched = xoTypeScript.filter(config => config.language?.startsWith('json/') !== true);

export default [
    {
        ignores: [
            'wwwroot/js/*.js'
        ]
    },
    ...xoTypeScriptPatched,
    {
        languageOptions: {
            parserOptions: {
                // https://github.com/typescript-eslint/typescript-eslint/issues/9739#issuecomment-2296442418
                projectService: {
                    allowDefaultProject: ['eslint.config.mjs'],

                    // Use of import.meta.dirname instead of ./ to fix config read error when opening a .ts file in VS although the change
                    // still does not fix broken linting in VS, see https://developercommunity.microsoft.com/t/Use-updated-flat-config-eslintconfigj/10703354
                    defaultProject: `${import.meta.dirname}/tsconfig.json`
                }
            }
        }
    },
    {
        rules: {
            'no-unused-vars': 'error',
            curly: ['error', 'multi-or-nest', 'consistent'],
            '@stylistic/padded-blocks': 'off',
            '@stylistic/indent': ['error', 4],
            '@stylistic/indent-binary-ops': ['error', 4],
            '@stylistic/comma-dangle': ['error', 'never'],
            '@stylistic/function-paren-newline': ['error', 'consistent'],
            '@stylistic/curly-newline': ['error', { minElements: 1 }],
            '@stylistic/object-curly-spacing': ['error', 'always'],
            '@typescript-eslint/no-empty-object-type': ['error', { allowInterfaces: 'with-single-extends' }]
        }
    },

    // Linting rule adjustments for this file
    {
        files: ['eslint.config.mjs'],
        rules: {
            '@typescript-eslint/no-unsafe-assignment': 'off',
            '@typescript-eslint/no-unsafe-call': 'off',
            '@typescript-eslint/no-unsafe-member-access': 'off',
            '@typescript-eslint/naming-convention': 'off'
        }
    }
];
