   M S B A M L   `   `   `  ����  MouldSpellCreator*    $MouldSpellCreator.NotificationWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � Title$  Notification Popup��5      $	��300��6    � SizeToContent$ Height=�6*    � WindowStyle$
 None=�5       � AllowsTransparency . 
������  � ShowInTaskbar .   � Topmost . .��5	      �  J� RenderTransformOrigin$	 0,1&�6   .+�5   
   �� $��1q�6   
�������
���   �._�5      �� $��20q�6   .+�5      �� -   5         ��Name$ Header��5      $��Heavy"�6,   $
��Wrap=�6?   $��5q�6S   �� 5      �� 6#   5      �� 5      -   5        $ Description��5      $��Heavy"�61   $
��Wrap=�6D   $��5q�6X   5      5      5       � Triggers 5   
   :� 5      	 :� RoutedEvent$	 FrameworkElement.Loaded��6   .[�5      �� .��5      �� .��5      U� 
 �� TargetProperty$9
 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)�68   .\�5      ��  O� KeyTime$ 0:0:0��63    O� Value$ 0Q�6C   �� 5      $ 0:0:0.5��63   $ 1Q�6E   5      U� 5       -   5        $
 (UIElement.Opacity)�5    8   .\�5!      �� $ 0:0:2��63   $ 1Q�6C   �� 5"      $ 0:0:4��63   $ 0Q�6C   5#      5$      5%      5&      z�5)   
   �� 5*       �� ScaleY$ 1Q�6   5-      5/      