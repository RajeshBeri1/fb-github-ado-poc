/**
 * Helper function to replace the swagger keywords with the correct values
 */
const replaceInFiles = require('replace-in-files');

// Get cli arguments
const path = process.argv.slice(2);
console.log('path: ', path);

const options = {
    // See more: https://www.npmjs.com/package/globby
    // Single file or glob
    files: 'path/to/file',
    // Multiple files or globs
    files: path,  
   
    // See more: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/replace
    // Replacement


    // _Default?: string = null;

    from: /import \{\s(.*?)\s\} from '(.*?)'.*\s*(?=export\sclass\s\1).*\{ /g,  // string or regex
    to: 'export class $1 {', // string or fn  (fn: carrying last argument - path to replaced file)   
    // See more: https://www.npmjs.com/package/glob
    optionsForFiles: { // default
      "ignore": [
        "**/node_modules/**"
      ]
    },   
    saveOldFile: false, // default        
    encoding: 'utf8',  // default   
    shouldSkipBinaryFiles: true, 
    onlyFindPathsWithoutReplace: false, 
    returnPaths: true,
    returnCountOfMatchesByPaths: true
  };

  replaceInFiles(options)
  .then(({ changedFiles, countOfMatchesByPaths }) => {
    console.log('Modified files:', changedFiles);
    console.log('Count of matches by paths:', countOfMatchesByPaths);
    console.log('was called with:', options);
  })
  .catch(error => {
    console.error('Error occurred:', error);
  });

