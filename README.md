# The Flappy Game - a Kinect Lock-in-Amplifier demo

A Windows Presentation Foundation (or WPF) C# Kinect game that demonstrates the application and equivalance of digital Lock-in Feedback (LiF) versus analog Lock-in Amplification (LiA).

The game invites the player to wave his or her arms in the rhytm of a line moving up and down the screen. The line speeds up if a player follows the rhytm closely. It slows down if the player is unable to follow the rhytm. You have 40 seconds to drive the speed of the line up as much as possible. The player will then be awarded a score based on the speed of the line at that very moment. 

Importantly, the speed of the line is determined by our (Davide Iannuzi, Maurits Kaptein, and Robin van Emden) [Lock-in Feedback algorithm](https://arxiv.org/abs/1502.00598), or, in the LiA version, by a live, analog Lock-in Amplifier ([Stanford Research Systems SR830](http://www.thinksrs.com/products/SR810830.htm)), as the following chart illustrates:

<img src="https://raw.githubusercontent.com/robinvanemden/Flappy---Kinect-Lock-in-Amplifier-demo/master/Images/flap.png" width="50%">

Both the LiA and the LiF versions of the game make use of the following (free) projects:

- The [Vitruvius](http://vitruviuskinect.com/) Kinect framework.
- [MakApps.Metro](https://github.com/MahApps/MahApps.Metro), a toolkit for creating Metro styled WPF apps.
- [OxyPlot](http://www.oxyplot.org/), a cross-platform plotting library for .NET.

Additionally, the LiA version makes use of a LabJack U3 (LV version) to interface the game with a Lock-in Amplifier. This version additionaly makes use of LabJack's Windows UD library, the high-level Windows library/driver for the LabJack U3, U6 and UE9:

- [LabJack](https://labjack.com/products/u3) U3 examples (the .NET assembly is a wrapper to the UD driver) can be found [here](https://labjack.com/support/software/examples/ud/dotnet)

For photos of the setup (right) and the game being played (left), see bellow.

<img src="https://raw.githubusercontent.com/robinvanemden/Flappy---Kinect-Lock-in-Amplifier-demo/master/Images/play.jpg" width="48%">  <img src="https://raw.githubusercontent.com/robinvanemden/Flappy---Kinect-Lock-in-Amplifier-demo/master/Images/setup.jpg" width="48%">

For a video of the game, see:

https://youtu.be/vN1kBffZOS8?list=PLfezfZWsuaJVeFtIvgYVxDs_uE5A6M8Xc
