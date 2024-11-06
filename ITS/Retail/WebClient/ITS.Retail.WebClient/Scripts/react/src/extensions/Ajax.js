
import _ from 'underscore';
//import Promise from 'bluebird';
/**
 * httpGet description with Promises
 * @param  {String} url - Get Url
 */
exports.httpGet = function (url) {
  // Return a new promise.
  return new Promise(function(resolve, reject) {
    // Do the usual XHR stuff
    var req = new XMLHttpRequest();
    req.open('GET', url);

    req.onload = function() {
      // This is called even on 404 etc
      // so check the status
      if (req.status == 200) {
        // Resolve the promise with the response text
        resolve(req.response);
      }
      else {
        // Otherwise reject with the status text
        // which will hopefully be a meaningful error
        reject(Error(req.statusText));
      }
    };

    // Handle network errors
    req.onerror = function() {
      reject(Error('Network Error'));
    };

    // Make the request
    req.send();
  });
}

/**
 * httpPost description
 * @param  {string} url - Post Url
 * @param  {array} data - Post data array
 * @param  {boolean} async - Asyncronous
 */
exports.httpPost = function(url, data, async) {
  return new Promise(
      function (resolve, reject) {
          var request = new XMLHttpRequest();

          request.onload = function () {

            if (request.status === 200) {
                // Success
                resolve(request.response);
            } else {
                // Something went wrong (404 etc.)
                reject(new Error(request.statusText));
            }
          }

          request.onerror = function () {
              reject(new Error(
                  'XMLHttpRequest Error: '+request.statusText));
          };

          let formData = new FormData();

          _.each(data, function(value, key){
              formData.append(key,value);
          });

          request.open('POST', url, async);
          request.send(formData);
      });
};
