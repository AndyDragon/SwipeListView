# SwipeListView for Runtime & Silverlight

Credits from AndyDragon
=======================
This is a fork of the the https://github.com/FrayxRulez/SwipeListView repository, the only addition I have made is to allow the swipe buttons to stay on screen (sticky). All credit goes to FrayxRulez for creating and implementing the swipe list view.

Introduction
============
This repository contains a list with items swiping feature, like the one in new Outlook Mail app for Windows 10.

![Demo](http://gifyu.com/images/ezgif-33935773781.gif)

In Runtime controls are based on `ListView` & `ListViewItem`, in Silverlight on `ListBox` & `ListBoxItem`. Items have two side options fully customizables via ItemX properties of their parent SwipeListView.

Available properties are:
* `ItemLeft/RightBackground`: Changes color of side items backgroud.
* `ItemLeft/RightTemplate`: DataTemplates of side items, content is the item, so all is in binding.
* `ItemLeft/RightBehavior`: Changes the behavior of side items.

Available behaviors are:
* `Collapse`: triggered at 2/5 of the width of the item. Once triggered side menu is collapsed and state is restored.
* `Expand`: triggered at 3/5 of the width of the item. Once triggered side menu is expanded, state is not restored.
* `Disabled`: side item is disabled.


You can handle swipe with `ItemSwipe` event. It is triggered at start of animation for `Collapse` behavior and at the end for `Expand`. Event arguments are `SwipedItem` and `Direction`.

The code is released under the MIT/X11, so feel free to modify and share your changes with the world.

How to
======
A demo project (Runtime) is included in solution.

Install
=======
You can install the library via [NuGet].

###Install-Package [SwipeListView]

[NuGet]:http://nuget.org/
[SwipeListView]:http://www.nuget.org/packages/SwipeListView
