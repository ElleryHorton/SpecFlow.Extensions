# SpecFlow.WebExtension
web extension methods for SpecFlow Web and Selenium

This project will be a container for the SpecFlow Web / Selenium extension code that I develop while
using said software tools. Too often I find myself looking up webdriver code on the internet and I'd
like to keep everything I use in one convenient and portable library (for future use of course).

# Framework

For starters, this is not a "plug-and-play one size fits all" test framework. This is a "modify and 
integrate into your test environment" test framework. That said, you'll most likely have to create
and fill in code to get portions of SpecFlow.Extensions to work for your particular situation. It is,
however, IMHO, a good place to start.

# A note about Page Objects, ByEx, and the new Find method:

I'm assuming that you're already familiar with and/or following the best practice of using Page Objects
for your SpecFlow Web or Selenium webdriver automation. That being the case: I worked at a company
where Page Objects contained public properties that returned IWebElements. For example, a LoginPage
object might look like:

<code><pre>
  class LoginPage
  {
    private IDriver _driver;
    LoginPage(IDriver driver)
    {
      _driver = driver;
    }
    public IWebElement Username { return _driver.FindElement(By.Id("username")); }
    public IWebElement Password { return _driver.FindElement(By.Id("password")); }
  }
</pre></code>

~~The problem with this is that a future attempt to implement retry logic was a nightmare. Because the
Page Object called FindElement, the ability to know which By was used to identify the IWebElement
was contained entirely in the Page Object.~~ Furthermore, in order to test the negative case (a
IWebElement is not found), a FindElementSafe method had to be implemented that would return null
if the underlying FindElement method threw an exception. More often than not, the null value that
was returned would inevitably wreck havoc somewhere else in the test code or result in cryptic
"Object not set to a reference" errors during runtime.

~~The problem is that the Page Object is fulfilling two duties. For one, the Page Object knows how to
identify the IWebElement and, two, the Page Object took it upon itself to retreive the IWebElement
for us. IMHO, the Page Object, like all classes, should only have one responsibility. That is, to
tell us how to identify an IWebElement.~~

<strong>Edit:</strong> I recommend the Page Object to return a ByEx for elements you want to negative test, check the existence of, or check the non-existence. Continue to use IWebElement, SelectElement, *Element, etc. for elements you expect to always be there and are required for the test to be meaningful. For example, a progress bar or fading message might be a good candidate for a ByEx property on a Page Object while a username and password field should be IWebElements since a test that couldn't login would be pretty meaningless.

To that end, I created a Find method that takes a ByEx (I like to call it the WebElementIdentifier).
Test code that access the Page Object's properties can then use the ByEx to Find, WaitFor, or make
sure that IWebElements do not exist. As an added benefit, the resulting Page Object is simpler:
<pre><code>
  class LoginPage
  {
    public ByEx Username { return new ByEx(By.Id("username")); }
    public ByEx Password { return new ByEx(By.Id("password")); }
  }
</code></pre>

Now, in the calling code, it is possible to (psuedo code):
  <pre><code>WaitFor(LoginPage.Username)</code></pre>
  or
  <pre><code>DoesNotExist(LoginPage.Username)</code></pre>
  
The Find method is also smarter in that it can use an appropriate FindElementBy* method depending on
the properties that are set on the ByEx. So far, text and one attribute is supported. I will add
support for multiple attributes in the near future.
