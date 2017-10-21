Get Started with Firebase Analytics for Android

Firebase Analytics collects usage and behavior data for your app. The SDK logs two primary types of information:

 - **Events**: What is happening in your app, such as user actions, system events, or errors.

 - **User properties**: Attributes you define to describe segments of your userbase, such as language preference or geographic location.

Analytics automatically logs some events and user properties; you don't need to add any code to enable them.



Configuring Firebase
--------------------

1. Create a Firebase project in the [Firebase Console][1], if you don't already have one setup.  If you have an existing Google project associated with your mobile app, you can use the **Import Google Project** option.  Otherwise, use the **Create New Project** option.

2. Click **Add Firebase to your Android app**.  

  a. Enter your app's ***Package Name***
  
  b. Enter the SHA-1 Hash for your signing certificate.  If you don't know how to find this, [check out this guide][2]

3. Click **Add App** and download the ***google-services.json*** file generated for you.

4. Add the ***google-services.json*** file to your Xamarin.Android project.

5. Set the *Build Action* for your ***google-services.json*** file to ***GoogleServicesJson***.  

The build process will take the api keys and values from your ***google-services.json*** and translate them into the correct resource string key/value pairs in your app.









### Add Analytics to your App

Declare the `FirebaseAnalytics` object at the top of your activity:

```csharp
FirebaseAnalytics firebaseAnalytics;
```

Then initialize it in the `OnCreate ()` method:

```csharp
// Obtain the FirebaseAnalytics instance.
firebaseAnalytics = FirebaseAnalytics.GetInstance (this);
```

### Log events

Once you have created a `FirebaseAnalytics` instance, you can use it to log either predefined or custom events with the `LogEvent ()` method. You can explore the predefined events and parameters in the `FirebaseAnalytics.Event` and `FirebaseAnalytics.Param` reference documentation.

The following code logs a `SelectContent` Event when a user clicks on a specific element in your app.

```csharp
var bundle = new Bundle`();
bundle.PutString (FirebaseAnalytics.Param.ItemId, id);
bundle.PutString (FirebaseAnalytics.Param.ItemName, name);
bundle.PutString (FirebaseAnalytics.Param.ContentType, "image");

firebaseAnalytics.LogEvent (FirebaseAnalytics.Event.SelectContent, bundle);
```

### Confirm Events

You can enable verbose logging to monitor logging of events by the SDK to help verify that events are being logged properly. This includes both automatically and manually logged events.

You can enable verbose logging with a series of adb commands:

```
adb shell setprop log.tag.FA VERBOSE
adb shell setprop log.tag.FA-SVC VERBOSE
adb logcat -v time -s FA FA-SVC
```

This command displays your events helping you immediately verify that events are being sent.




Samples
=======

You can find a Sample Application within each Firebase component.  The sample will demonstrate the necessary configuration and some basic API usages.






Learn More
==========

You can learn more about the various Firebase SDKs & APIs by visiting the official [Firebase][5] documentation




[1]: https://console.developers.google.com/ "Google Developers Console"
[2]: https://developer.xamarin.com/guides/android/deployment,_testing,_and_metrics/MD5_SHA1/ "Finding your SHA-1 Fingerprints"
[3]: https://developers.google.com/android/ "Google APIs for Android"
[4]: https://firebase.google.com/console/ "Firebase Developer Console"
[5]: https://firebase.google.com/ "Firebase"
