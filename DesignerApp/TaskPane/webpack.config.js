/* eslint-disable no-undef */

const devCerts = require('office-addin-dev-certs');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const webpack = require('webpack');

const urlDev = 'https://mcs-dev-designerapp-eastus.yellowhill-44cae165.eastus.azurecontainerapps.io';
const urlProd = 'https://www.contoso.com/'; // CHANGE THIS TO YOUR PRODUCTION DEPLOYMENT LOCATION

async function getHttpsOptions() {
    const httpsOptions = await devCerts.getHttpsServerOptions();
    return {
        cacert: httpsOptions.ca,
        key: httpsOptions.key,
        cert: httpsOptions.cert,
    };
}

module.exports = async (env, options) => {
    const dev = options.mode === 'development';
    const httpsOptions = await getHttpsOptions();
    const config = {
        devtool: 'source-map',
        entry: {
            polyfill: ['core-js/stable', 'regenerator-runtime/runtime'],
            vendor: ['react', 'react-dom', 'core-js', '@fluentui/react'],
            taskpane: ['./src/taskpane/index.tsx'], // Removed 'react-hot-loader/patch'
            commands: './src/commands/commands.ts',
        },
        output: {
            clean: true,
        },
        resolve: {
            extensions: ['.ts', '.tsx', '.html', '.js', '.css', '.json'],
        },
        module: {
            rules: [
                {
                    test: /\.(ts|tsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                        options: {
                            presets: [
                                '@babel/preset-env',
                                ['@babel/preset-react', { runtime: 'automatic' }],
                                '@babel/preset-typescript',
                            ], // Removed 'plugins: ['react-hot-loader/babel']'
                        },
                    },
                },
                {
                    test: /\.html$/,
                    exclude: /node_modules/,
                    use: 'html-loader',
                },
                {
                    test: /\.(png|jpg|jpeg|gif|ico)$/,
                    type: 'asset/resource',
                    generator: {
                        filename: 'assets/[name][ext][query]',
                    },
                },
                {
                    test: /\.css$/i,
                    use: ['style-loader', 'css-loader'],
                },
            ],
        },
        plugins: [
            new CopyWebpackPlugin({
                patterns: [
                    {
                        from: 'assets/',
                        to: 'assets/',
                    },
                    {
                        from: 'manifest*.xml',
                        to: '[name]' + '[ext]',
                        transform(content) {
                            if (dev) {
                                return content;
                            } else {
                                return content
                                    .toString()
                                    .replace(new RegExp(urlDev, 'g'), urlProd);
                            }
                        },
                    },
                ],
            }),
            new HtmlWebpackPlugin({
                filename: 'taskpane.html',
                template: './src/taskpane/taskpane.html',
                chunks: ['taskpane', 'vendor', 'polyfills'],
            }),
            new HtmlWebpackPlugin({
                filename: 'index.html',
                template: './src/taskpane/taskpane.html',
                chunks: ['taskpane', 'vendor', 'polyfills'],
            }),
            new HtmlWebpackPlugin({
                filename: 'commands.html',
                template: './src/commands/commands.html',
                chunks: ['commands'],
            }),
            new webpack.ProvidePlugin({
                Promise: ['es6-promise', 'Promise'],
            }),
        ],
        devServer: {
            hot: true,
            headers: {
                'Access-Control-Allow-Origin': '*',
            },
            historyApiFallback: true,
            server: {
                type: 'https',
                options: httpsOptions,
            },
            port: process.env.npm_package_config_dev_server_port || 3000,
        },
    };

    return config;
};