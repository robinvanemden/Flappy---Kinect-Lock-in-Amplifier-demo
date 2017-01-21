# Flappy - Kinect Lock-in-Amplifier demo

A Microsoft Windows Presentation Foundation (WPF) C# Kinect game that demonstrates and compares digital Lock-in Feedback (LiF) versus analog Lock-in Amplification (LiA).

# Description

"Flappy Bird" makes use of an [XBox Kinect motion sensing input device](https://en.wikipedia.org/wiki/Kinect) to demonstrate the equivalence of the digital Lock-in Feedback (LiF) algorithm to an analog Lock-in Amplification (LiA). To do so, "Flappy" invites a player to wave his or her arms in a rhythm that is set by a line that moves up and down the screen. The line speeds up if the player is able to follow the line closely. It slows down if he or she is unable to keep up. The goal of the game is to make the line move as fast as possible in a limited period of time.

Importantly, the speed with which the line moves up and down the screen is determined by our Lock-in Feedback algorithm, or, in the LiA version, by a live, analog Lock-in Amplifier (Stanford Research Systems SR830). 

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

# Software libraries and frameworks

Both the LiA and the LiF versions of the game make use of the following (free) libraries and frameworks:

- The [Vitruvius](http://vitruviuskinect.com/) Kinect framework.
- [MakApps.Metro](https://github.com/MahApps/MahApps.Metro), a toolkit for creating Metro styled WPF apps.
- [OxyPlot](http://www.oxyplot.org/), a cross-platform plotting library for .NET.

Additionally, the LiA version makes use of a LabJack U3 (LV version) to interface the game with the Lock-in Amplifier:

- [LabJack](https://labjack.com/products/u3) U3 examples (the .NET assembly is a wrapper to the UD driver) can be found [here](https://labjack.com/support/software/examples/ud/dotnet)
