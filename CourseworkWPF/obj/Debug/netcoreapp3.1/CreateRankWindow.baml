   M S B A M L   `   `   `  ����H  DCourseworkWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CourseworkWPFCourseworkWPF  5   	   $    CourseworkWPF.CreateRankWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:CourseworkWPF     5   	     � Title$  CreateRankWindow��5   	   $	��300��6"   $	��500��6/   .��5	      � .+�5
   
   �� $��
0,0,251,69q�6   $��3q�6%   
������._�5      � -   5         �Name$ imageRankPanel��5      
�������$��5,5,5,5q�6B   .+�5      �� -   5         ��Name$ noImageFoundLabel��5   ^   $��Center=�6   $��Center=�65   $��20(�6P   $��NO IMAGE FOUND��6y   5      5      �� 5   
   $��254,4,11,254q�6   $��	Rank Name��6&   �� 5   
   -   5         ��Name$ rankNameTextBox��5   I   $��254,37,11,217q�6    J� RenderTransformOrigin$ 0.5,0.5&�6)   .��6b   z�5      V� 5      .~�5      �� �� 5       �� AngleX$ 0Q�6$   �� 5      T� 5       T� X$ 0Q�6)   5      .��5   '   5      �� 5   
   $��254,76,11,182q�6   $��	Rank Type��6'   �� 5   
   -   5         ��Name$ rankTypeComboBox��5   +   $��254,109,11,145q�6   .@�5      �� $��Enlisted��6   �� 5      $��Non Commissioned��6   �� 5      $��Warrant��6   �� 5       $��Officer��6   5!      �� 5"   
   $��254,152,11,106q�6   $��
Rank Value��6(   �� 5#   
   -   5        $ rankValueTextBox��5#   I   $��254,185,11,69q�6   $ 0.5,0.5&�6)   .��6c   z�5$      V� 5%      .~�5&      �� �� 5'      $ 0Q�6$   �� 5(      T� 5)      $ 0Q�6)   5*      .��5+   '   5-      �� 5.   
   -   5         ��Name$ createRankButton��5.      $��52,239,298,10q�6+   $��Create��6C   6T   �� 5/   
   -   5        $ resetRankButton��5/      $��296,239,54,10q�6*   $��Reset��6A   6Q   50      51      