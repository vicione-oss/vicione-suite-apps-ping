import path from 'node:path';
import eslintConfigXo from 'eslint-config-xo';

const indent = 4;

const config = [
    {
        ignores: [
            'wwwroot/js/*.js',
            '**/*.css',
            '**/*.json'
        ]
    },
    ...eslintConfigXo({ space: indent }),
    {
        languageOptions: {
            parserOptions: {
                // https://github.com/typescript-eslint/typescript-eslint/issues/9739#issuecomment-2296442418
                projectService: {
                    allowDefaultProject: ['eslint.config.mjs'],

                    // Use of import.meta.dirname instead of ./ to fix config read error when opening a .ts file in VS although the change
                    // still does not fix broken linting in VS, see https://developercommunity.microsoft.com/t/Use-updated-flat-config-eslintconfigj/10703354
                    defaultProject: path.join(import.meta.dirname, 'tsconfig.json')
                }
            }
        }
    },
    {
        rules: {
            'no-unused-vars': 'error',
            curly: ['error', 'multi-or-nest', 'consistent'],
            '@stylistic/padded-blocks': 'off',
            '@stylistic/indent': ['error', indent],
            '@stylistic/indent-binary-ops': ['error', indent],
            '@stylistic/comma-dangle': ['error', 'never'],
            '@stylistic/function-paren-newline': ['error', 'consistent'],
            '@stylistic/curly-newline': ['error', { minElements: 1 }],
            '@stylistic/object-curly-spacing': ['error', 'always'],
            '@typescript-eslint/no-empty-object-type': ['error', { allowInterfaces: 'with-single-extends' }],

            // Files and directories are named after the .NET types and projects they belong to
            'unicorn/filename-case': 'off'
        }
    },

    // Linting rule adjustments for this file
    {
        files: ['eslint.config.mjs'],
        rules: {
            // The rule cannot read `package.json` because of its byte order mark and reports every import as undeclared
            'import-x/no-extraneous-dependencies': 'off'
        }
    }
];

export default config;
