import messages from './messages';

/**
 * Class Providing Locale messages per language
 */
export default class Locale{
   /**
    * Initialize this.lang and this.message
    */
  constructor() {
    this.lang = lang;
    this.message = message;
  }
  /**
   * Get Message with lang and message name and return the localized message
   * @param  {String} lang    Language Locale
   * @param  {String} message Message
   * @return {String}         Returned Message
   */
  static getMessage(lang,message) {
    return messages.values[lang][message];
  }
}
