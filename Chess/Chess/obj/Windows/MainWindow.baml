   M S B A M L   `   `   `  ���� @  <Chess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ChessChess      Chess.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:Chess       � Title$  Chess��$	��800��$	��800��.��� -   .+�� -   $��Board��$��2�� � ColumnDefinitions �� �� �� �� �� �� �� ��  � RowDefinitions �� �� �� �� �� �� �� �� ����  �� ImageSource$ ./../Images/Board.png��� -   $��PromotionPopup��$��Mouse=� � HorizontalOffset$
 -100����.   � VerticalOffset$
 -100�� � AllowsTransparency . .��� -   $��PromotionGrid�� �� �� �� �� .+�A� $	��600��$	��300��
�������$��4���� -   $��50,0,0,0q�
 � Tag$
 Rook��$��0��$
��Left=�$	��100��$	��100��
���������Z� .���� $(��"Images/DualTypeImages/DualRook.png���� -   $ Bishop��$��1��$
��Left=�$	��100��$	��100��
���������Z� .���� $*��$Images/DualTypeImages/DualBishop.png���� -   $ Knight��$��2��$
��Left=�$	��100��$	��100��
���������Z� .���� $*��$Images/DualTypeImages/DualKnight.png���� -   $ Queen��$��3��$
��Left=�$	��100��$	��100��
���������Z� .���� $)��#Images/DualTypeImages/DualQueen.png������  �� 
TargetType" ��.��� 	 �� Property	 � ��
 �� Value

 ����� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ��� 	 � ��

 ������ -	   $��GameOver��$��Mouse=�$
 -100����.  $
 -100�� . .���  �� �� �� .+�A� $	��800��$	��400��
�������$��4���� $��50q�$��30(�$��0��$	��290��$	��120��	Gameover?�� The game might have�� more positions. . .�� -
   $��26(�$��1��$��1��$
��Left=�$	��150��$	��150��
��� � ���Z� .��	Continue.�� -   $��26(�$��2��$
��Left=�$	��150��$	��150��
���  ����Z� .��Exit.���� " ��.��� 	 � ��

 ����� P�  �@J�IsMouseOver . . ��� 	 � ��

 �����