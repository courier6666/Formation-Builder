   M S B A M L   `   `   `  ����H  DCourseworkWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CourseworkWPFCourseworkWPF  5   	   )    #CourseworkWPF.FormationCreateWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:CourseworkWPF     5   	     � Title$  FormationCreateWindow��5   	   $	��450��6'   $	��500��64   .��5	      � $��0,0,0,0q�6   .+�5
   
   �� $��0,0,250,217q�6   $��3q�6&   
������._�5      � -   5        $��
imagePanel��5      
�������$��5,5,5,5q�6<   .+�5      �� -   5        $��noImageFoundLabel��5   ^   $��Center=�6   $��Center=�65   $��20(�6P   .��6w   NO IMAGE FOUND5      5      5      �� 5   
   $��255,6,10,400q�6   .��6&   Formation Type5      �� 5   
   -   5        $��formationTypeComboBox��5   *   $��255,39,10,363q�6   .@�5      �� $��	Formation��6   5      �� 5      $��Lowest Unit��6   5      5      �� 5   
   $��255,76,10,330q�6   $��Formation Name��6'   �� 5   
   -   5        $��formationNameTextBox��5   J   $��255,109,10,293q�6    J� RenderTransformOrigin$ 0.5,0.5&�6*   .��6f   z�5      V� 5      .~�5       �� �� 5!       �� AngleX$ 0Q�6$   �� 5"      T� 5#       T� X$ 0Q�6)   5$      .��5%   '   5'      �� 5(   
   $��255,141,10,265q�6   $��Hierarchy Name��6(   �� 5)   
   -   5        $��hierarchyNameTextBox��5)   J   $��255,174,10,228q�6   $ 0.5,0.5&�6*   .��6g   z�5*      V� 5+      .~�5,      �� �� 5-      $ 0Q�6$   �� 5.      T� 5/      $ 0Q�6)   50      .��51   '   53      �� 54   
   $��10,224,250,182q�6   $��Commander Rank��6(   �� 55   
   -   5        $��commanderRankTextBox��55   J   $��10,257,250,145q�6   $ 0.5,0.5&�6*   .��6g   z�56      V� 57      .~�58      �� �� 59      $ 0Q�6$   �� 5:      T� 5;      $ 0Q�6)   5<      .��5=   '   5?      �� 5@   
   $��10,299,250,107q�6   $,��&Heavy equipment. Example:M4, AK-47,...��6(   �� 5A   
   -   5        $"��heavyEquipmentAsignedTextBox��5A   I   $��10,332,250,70q�6   $ 0.5,0.5&�6)   .��6n   z�5B      V� 5C      .~�5D      �� �� 5E      $ 0Q�6$   �� 5F      T� 5G      $ 0Q�6)   5H      .��5I   '   5K      �� 5L   
   $��255,299,10,107q�6   $��Additional Equipment��6(   �� 5M   
   -   5        $(��"AdditionalEquipmentAssignedTextBox��5M   I   $��255,332,10,70q�6   $ 0.5,0.5&�6)   .��6t   z�5N      V� 5O      .~�5P      �� �� 5Q      $ 0Q�6$   �� 5R      T� 5S      $ 0Q�6)   5T      .��5U   '   5W      �� 5X   
   -	   5         ��Name$ createFormationButton��5X      $��60,384,290,15q�60   .��6k   Create5Z      �� 5[   
   -
   5        $ resetFormationButton��5[      $��305,384,45,15q�6/   $��Reset��6F   6V   5\      5^      