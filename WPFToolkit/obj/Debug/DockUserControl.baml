   M S B A M L   `   `   `  ����E  AWPFToolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:WPFToolkit
WPFToolkit  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5           WPFToolkit.DockUserControl   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      *localclr-namespace:WPFToolkit     5      X TXceed.Wpf.Toolkit, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      i eXceed.Wpf.AvalonDock.Themes.VS2010, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4g cXceed.Wpf.AvalonDock.Themes.Aero, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4h	 dXceed.Wpf.AvalonDock.Themes.Metro, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4[
 WXceed.Wpf.AvalonDock, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4=xcad,http://schemas.xceed.com/wpf/xaml/avalondock   	 
 5      4s&clr-namespace:System;assembly=mscorlib   	 
 5      .��5      �   � RowDefinitions  5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   .+�5   
   ) 
 #Xceed.Wpf.AvalonDock.DockingManager  $��1��6   $
��1000��6+      AllowMixedOrientation . 
���   �$��1q�5         Theme 5      ��  �� ElementName$ _themeCombo�� �� Path$ SelectedItem.Tag�   DocumentHeaderTemplate 5      �� 5      .R�5      ��  �� Orientation$ 
Horizontal=�6!   .+�5      �� ��6$   �� *Title+5      5         Layout. 5      , 
 &Xceed.Wpf.AvalonDock.Layout.LayoutRoot  -   5        _layoutRoot��   	RootPanel. 5      - 
 'Xceed.Wpf.AvalonDock.Layout.LayoutPanel  	   Orientation$	 
Horizontal=�6#   � 
 �Xceed.Wpf.AvalonDock.Layout.LayoutGroup`1[[Xceed.Wpf.AvalonDock.Layout.ILayoutPanelElement, Xceed.Wpf.AvalonDock, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4]]
   Children.
 5      6 
 0Xceed.Wpf.AvalonDock.Layout.LayoutAnchorablePane  � 
 �Xceed.Wpf.AvalonDock.Layout.LayoutPositionableGroup`1[[Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable, Xceed.Wpf.AvalonDock, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4]]   	DockWidth$	 150 �60   � 
 �Xceed.Wpf.AvalonDock.Layout.LayoutGroup`1[[Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable, Xceed.Wpf.AvalonDock, Version=2.9.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4]]   Children. 5      2 
 ,Xceed.Wpf.AvalonDock.Layout.LayoutAnchorable  /	 
 )Xceed.Wpf.AvalonDock.Layout.LayoutContent 	  	ContentId$ 
properties��60    	  Title$ 
Properties��6G      CanHide .   	  CanClose .     AutoHideWidth$	 240Q�5   -    	  Content. 5      �   5   "   �� 5    &   $
��Auto �64   6D   �� 5!   &   $
��Auto �64   6D   �� 5"   &   $
��Auto �64   6D   �� 5#   &   $��* �64   6A   .+�5%   "   �� -   5        $	��Add��5%   =   $��20��6K   $��5q�6W   $	��Top=�6b   6|   �� 5&   "   -   5        $��1��5&   D   $��Clear��6Q   $��20��6a   $��5q�6m   $	��Top=�6x   6�   #
   WPFToolkit.WidthScrollControl
  5'   "   -   5         
 Name$ widthScrollControl��5'   ;   $��2��6W   6f      WPFToolkit.PaletteControl  5(   "   -   5          Name$ paletteControl��5(   7   $��3��6P   6_   5)      5*      5+      4 
 .Xceed.Wpf.AvalonDock.Layout.LayoutDocumentPane  5-      -   5        documentPane��5.      50      52      53      54      55      