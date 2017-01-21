# The Flappy Game - a Kinect Lock-in-Amplifier demo

A Microsoft Windows Presentation Foundation (WPF) C# Kinect game that demonstrates the application and equivalence of digital Lock-in Feedback (LiF) versus analog Lock-in Amplification (LiA).

# Description

The game invites the player to wave his or her arms in the rhythm of a line moving up and down the screen. The line speeds up if a player follows the rhythm closely. It slows down if the player is unable to follow the rhythm. You have 40 seconds to drive the speed of the line up as much as possible. The player will then be awarded a score based on the speed of the line at that very moment. 

# Illustration

Photos of the analog LiA setup (to the left - lab equipment of course unnecessary for the digital LiF version) and the game being played (right):

<img src="https://raw.githubusercontent.com/robinvanemden/Flappy---Kinect-Lock-in-Amplifier-demo/master/Images/setup.jpg" width="48%">   <img src="https://raw.githubusercontent.com/robinvanemden/Flappy---Kinect-Lock-in-Amplifier-demo/master/Images/play.jpg" width="48%">

For some videos of the game in action, see:

https://youtu.be/vN1kBffZOS8?list=PLfezfZWsuaJVeFtIvgYVxDs_uE5A6M8Xc

# Background

The speed of the line is determined by our (Davide Iannuzzi, Maurits Kaptein, and Robin van Emden) [Lock-in Feedback algorithm](https://arxiv.org/abs/1502.00598), or, in the LiA version, by a live, analog Lock-in Amplifier ([Stanford Research Systems SR830](http://www.thinksrs.com/products/SR810830.htm)). The principle is illustrated by the following chart:

<img src="https://raw.githubusercontent.com/robinvanemden/Flappy---Kinect-Lock-in-Amplifier-demo/master/Images/flap.png" width="50%">

# Acknowledgments

The game has been made possible by the generous support of the Foundation for Fundamental Research on Matter ([FOM](https://www.fom.nl/en/)) and the Netherlands Organisation for Scientific Research ([NWO](http://www.nwo.nl/en)). It was presented at the yearly Dutch [Physics@Veldhoven](https://www.fom.nl/agenda/physicsatveldhoven/information/) (2017) conference.

# Libraries

Both the LiA and the LiF versions of the game make use of the following (free) projects:

- The [Vitruvius](http://vitruviuskinect.com/) Kinect framework.
- [MakApps.Metro](https://github.com/MahApps/MahApps.Metro), a toolkit for creating Metro styled WPF apps.
- [OxyPlot](http://www.oxyplot.org/), a cross-platform plotting library for .NET.

Additionally, the LiA version makes use of a LabJack U3 (LV version) to interface the game with the Lock-in Amplifier:

- [LabJack](https://labjack.com/products/u3) U3 examples (the .NET assembly is a wrapper to the UD driver) can be found [here](https://labjack.com/support/software/examples/ud/dotnet)
