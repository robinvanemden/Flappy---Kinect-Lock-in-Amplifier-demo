   M S B A M L   `   `   `  ���� T  PMahApps.Metro, Version=1.4.1.0, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0>.clr-namespace:OxyPlot.Wpf;assembly=OxyPlot.WpfOxyPlot.Wpf M IKinectHandTracking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:KinectHandTrackingKinectHandTracking #   KinectHandTracking.MainWindow   Hcontrols;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro  \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     >oxy.clr-namespace:OxyPlot.Wpf;assembly=OxyPlot.Wpf     2local clr-namespace:KinectHandTracking     -   (   "MahApps.Metro.Controls.MetroWindow   Name$  	AppWindow��
���   � � WindowStyle$
 None=� � WindowState$ 	Maximized=�   UseNoneWindowStyle .  � Title$ Kinect 2 hand tracking��$	��800��$
��1200��.��� .+�(� .��� .+��� -    ��Name$ HighBar��
������ $��Right=�$��9��$��
0,329,36,0q�$	��Top=�$��30���� -    ��Name$ Camera��$��0q��� -    ��Name$ Canvas��$��0q��� -   $ CanvasRight��.+��� -   $��AnimatedLine��
 �� Top$	 400��	 �� Left$	 0��$��Stretch=�$
��2500��
���  ��$��8��$y�Visible=�
 � 	Resources
  ��Name   Sbr �� 
TargetName g� RepeatBehavior g�Name �� TargetProperty W� From	 W� To g� Duration g� AutoReverse g� 
SpeedRatio��%�   &	        �� $	 sbr��$ AnimatedLine��$ Forever��.��W� $ da_right��$ (Canvas.Top)�$	 300X�$	 600X�$ 0:0:1D� . $ 1Q�A� -   $	��Ell��$y�Hidden=�$	 400��$	 40��
������ $��6��
���  ��$��-52,-52,0,0q�$	��100��$	��100��
   Sbel��%�   &	       �� $
 sbel��$	 Ell��$ Forever��.��W� $	 dal��$ (Canvas.Top)�$	 300X�$	 600X�$ 0:0:1D� . $ 1Q�A� -   $	��Elr��$y�Hidden=�$	 400��$	 40��
������ $��6��
���  ��$��-52,-52,0,0q�$	��100��$	��100��
   Sber��%�   &	       �� $
 sber��$	 Elr��$ Forever��.��W� $	 dar��$ (Canvas.Top)�$	 300X�$	 600X�$ 0:0:1D� . $ 1Q�� $��0��$��-6,566,684,-2q�.+���  �� Orientation$ 
Horizontal=�$
��Left=�$��30,-29,-140,29q�$	��624��.+��� $	��163��$��	0,94,0,12q� J� Opacity$	 0.7Q�����  �� Color$ Green�� �� Opacity$	 0.7Q�._���  �� TextAlignment$ Center=�$��Center=�$��Center=�$��48(��� -	    ��Name$ 	TblScore1��$�-���� $	��172��$��10,94,10,12q�$	 0.7Q����� $
 Blue��$	 0.7Q�._��� $ Center=�$��Center=�$��Center=�$��48(�$��22,21,22,20q��� -
   $ 	TblScore2��$�-���� $	��240��$��	0,27,0,12q�$	 0.7Q����� $	 Red��$	 0.7Q�._��� $ Center=�$��Center=�$��Center=�$��72(�$
��Bold"�
������� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect  �� -   $ 	TblScore3��$�-��  OxyPlot.Wpf.Plot  -     Name$ Plot1��$��24,36,738,552q�$	 0.8Q�
���   �   Title �� *Title+   Series   OxyPlot.Wpf.LineSeries  ���� *Points+  ���� *	PointsTwo+� -    �Name$ ScoreBar��$��Right=�$
��1000��$��36,36,36,36q�$��Stretch=�$��60��  � Orientation$  Vertical=�! � Maximum$	! 300Q�" � Minimum$
" -100Q�$	 0.7Q��� -   # ��Name$# CountDownText��$��Right=�$
��Wrap=�$��30��$	��Top=�$	��189��$	��307��$	��130(�
�������$
��Bold"�   &   System.Windows.Controls.DataGrid  -   $  Name$$ DataGrid��$y�Hidden=�%   EnableRowVirtualization% .  &   CanUserSortColumns& .  '   
IsReadOnly' . $��150,200,100,100q�$��36(�( � UseLayoutRounding( . )   MinColumnWidth$	) 200Q�$
��Bold"����	#7F060606   �� -   $# HighScoreGridTitle��$��Center=�$
��Wrap=�$��
Highscores��$	��Top=�$	��189��$	��456��$��80(�
�������$
��Bold"�$��368,36,368,0q�$y�Hidden=�   �� -   $ 
ImageClose��$y�Hidden=�* �� BitmapScalingMode$* HighQuality=�$��60��$��60��$��Content/close.png��$��Right=�$	��Top=�$��
0,70,100,0q�   �� -   + ��Name$+ 
StackPanel��$y�Hidden=�$��0,184,402,270q�$��Right=�$	��459�����	#FF5F5FE0   .+��� -   $# 	YouScored��$��You scored 9999 points!��$
��Bold"�$��30(�$��
25,25,25,0q�$ Center=�
��������� $��Enter your name below:��$
��Bold"�$��30(�$��	25,0,25,0q�$ Center=�
��������� -   , ��Name$, ResponseTextBox��$
��Bold"�$��30(�$��60��$��
25,25,25,0q�- �� TextAlignment$- Center=�
��������� -   $��Continue��$
��Bold"�$��30(�$��
25,25,25,0q�$��62��