   M S B A M L   `   `   `  ����  KinectStreams!    KinectStreams.CaptureWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$%  Kinect | Color, Depth, Infrared��5   	   $
��1080��61   $
��1920��6?    � WindowStartupLocation$ CenterScreen=�6z   .��5      � .+�5   
   �� -   5        $��camera��5      �� 5   
   -   5        $��canvas��5      � 5   
      GridButtonsStyle#��[  6   .+�5	      ��   StackPanelButtonsStyle#��[ 6   .+�5
      �� -   5         ��Name$ btnColorCam��5
      $��Color��6.     ButtonStyle#��[ 6>   6c   �� 5      -   5        $ btnDepthCam��5      $��Depth��6.   #��[ 6>   6c   �� 5      -   5        $ btnInfraredCam��5      $��Infrared��61   #��[ 6D   6i   �� 5      -   5        $ 
btnBodyCam��5      $
��Body��6-   #��[ 6<   6a   5      	� 5      -   5         	�Name$ 
btnLeftArm��5      $��Left Arm��6.   $
��Left=�6A   $��1216,22,0,0q�6\   $	��Top=�6q   6�   	� 5      -	   5        $ btnRightArm��5      $��	Right Arm��6/   $
��Left=�6C   $��1309,22,0,0q�6^   $	��Top=�6s   6�   �� 5      -
   5         ��Name$ txtUpperToForearmTheta��5      $
��Left=�66   $��23��6Q   $��
571,17,0,0q�6]    �� TextWrapping$
 Wrap=�6q   $��Theta��6�   $	��Top=�6�   $	��120��6�   6�   �� 5      -   5        $ 	RestTimer��5      $
��Left=�6)   $��23��6D   $��	19,17,0,0q�6P   $
 Wrap=�6c   $��TextBox��6w   $	��Top=�6�   $	��120��6�   �� 5      -   5        $ txtShoulderX��5      $
��Left=�6,   $��23��6G   $��
162,17,0,0q�6S   $
 Wrap=�6g   $��TextBox��6{   $	��Top=�6�   $��66��6�   �� 5      -   5        $ txtShoulderY��5      $
��Left=�6,   $��23��6G   $��
242,17,0,0q�6S   $
 Wrap=�6g   $��TextBox��6{   $	��Top=�6�   $��66��6�   �� 5      -   5        $ 	txtElbowX��5      $
��Left=�6)   $��23��6D   $��
325,17,0,0q�6P   $
 Wrap=�6d   $��TextBox��6x   $	��Top=�6�   $��66��6�   �� 5      -   5        $ 	txtElbowY��5      $
��Left=�6)   $��23��6D   $��
405,17,0,0q�6P   $
 Wrap=�6d   $��TextBox��6x   $	��Top=�6�   $��66��6�   5      5      5      