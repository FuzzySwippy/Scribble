# ToDo

## Alpha 0.1.0

- [X] Make the Canvas use chunks instead of being a single mesh
- [X] Fix the zoomed in snapping issue (Disabled the broken mouse cursor warping)
- [x] Layer entry context menu
- [X] Layer Editor
- [X] Add application menu at the top (File, Edit, etc...)
- [X] Add 'New' application menu entry with ability to specify resolution
- [X] Add image saving and loading to a custom format .src
- [X] Add image saving and loading to common formats (png, jpg, etc...)
- [X] Fix image disappearing when zooming in on large images
- [X] Add default file name in native dialogs for exported images
- [X] Middle click to sample color
- [X] Add missing icons (Error, warning, settings, duplicate, merge down, move up/down)
- [X] Make tool panel functional
- [X] Add line tool (click in one place and click in another to draw line)
- [X] Add rectangle selection tool
- [X] Add ability to move selected pixels
- [X] Add tool effect area preview overlay special layer
- [X] Add selection drawing tool
- [X] Only draw in selection
- [X] Add rectangle drawing tool
- [X] Add basic tools
- [X] Add history
- [X] Add undo/redo
- [X] Fix the save dialog showing up after selecting

## Alpha 0.2.0

- [X] Add missing icons in history
- [X] Fix bug where creating a new canvas doesnt clear history
- [X] Crop empty pixels in the layer visibility icon
- [X] Add image vertical and horizontal flipping
- [X] Add image rotation
- [X] Add canvas resizing
- [X] Optimize undo/redo data storage
- [X] Fix framerate dropping when moving cursor on large canvases
- [X] Optimize updates on large canvases
- [X] Add Ctrl+S shortcut for saving
- [X] Fix a line being drawn from the border when starting to draw after the window was unfocused
- [X] Made selected layer not render if visible is set to false even if its selected
- [X] Add crop to content (top/bottom, left/right, all sides)
- [X] Autosaving
- [X] Fix the save/export file dialog not opening the correct folder after opening image
- [X] Settings
- [X] History size setting (default 250)
- [X] Autosave interval setting (default 5 minutes)
- [X] Fix rotation not working on non-square images
- [X] Fix cropping to content only cropping to selection (or in the case of a drawing tool being selected, with a selection active, not cropping at all)
- [X] Fix history not registering drawing when started from a window that is being dismissed
- [X] Tool shortcuts with tooltips
- [X] Add About page

## Alpha 0.2.2

- [X] Add cut/copy/paste

## Alpha 0.3.0

- [X] Add cut, paste and crop to content history icons
- [X] Fix copying and cutting ignoring selection and copying the the entire square area
- [X] Selection size is off by one pixel (16x16 is shown as 15x15)
- [X] Exported/saved images with selection save selection color
- [X] Add grid
- [X] Add selection deselection to the rectangle select tool

## Alpha 0.3.1

- [X] Fix missing grabber in the hue slider
- [X] Add Color Selector for picking specific colors in menus
- [X] Deselect when using the move tool
- [X] Fix undoing moving single pixels keeps the selection of the moved pixels
- [X] Fix line tool having line offset when drawing from top right to bottom left
- [X] Export scaled images

## Alpha 0.3.2

- [X] Add shortcut text to context menu buttons
- [X] Add shortcuts for all menu bar items
- [X] Rework shortcuts
- [X] Add too large image check and error message
- [X] Fix scaled export X slider scale when width is less than height

## Alpha 0.4.0

- [X] Add magic selection tool
- [X] Tool button scaling to fit the tool panel when the content scale is large
- [X] Tool properties panel under the tool panel.
- [X] Add flood and magic selection color threshold property
- [X] Pencil draw area preview (toggleable in the settings)
- [X] Add header to the Scribble (.scrbl) format
- [X] The quickpencil popup color selector's colors aren't correctly shown on startup
- [X] Log autosaves
- [X] Hex color input doesnt update the color box when a value is pasted
- [X] Clear EffectAreaOverlay when the tool is changed
- [X] Add more attention grabbing notifications (slide out from the top)
- [X] Fix layers getting mixed up when moving them, undoing, or changing visibility (Made layer ids less likely to collide)
- [X] Pressing the close button several times opens multiple save request dialogs
- [X] Dither Tool

## Alpha 0.4.1

- [X] Fix memory leak when using pencil preview
- [X] Add previews for tools that are missing them

## Alpha 0.4.2

- [X] Fixed grid not showing up properly
- [X] Optimize canvas rendering
- [X] Fix after moving a selection the selection is deselected
- [X] Implement multithreading
- [X] Fix pencil preview flickering
- [X] Fix artifacts when using the flood tool on a large canvas
- [X] Fix corrupt images when saving
- [X] Fix windows defender detecting the application as a false positive
- [X] Magic selection tool diagonal property
- [X] Magic selection tool merge layers property
- [X] Flood selection tool diagonal property
- [X] Flood selection tool merge layers property
- [X] Paste in the same position as copied from (not relative to mouse)

## Alpha 0.4.3

- [X] Fix crash when using magic selection tool and copy pasting
- [X] Fix status bar not displaying any text
- [X] Fix status bar resolution not updating when resizing or cropping the canvas
- [X] Center notifications after changing the window size
- [X] Change default colors in quick pencils

## Alpha 0.4.4

- [X] Add selection rotation tool
- [X] Add selection rotation tool properties
- [X] Add ignore empty colors property to the selection rotation tool
- [X] Delete to delete the selection on the current layer
- [X] Deselect selection with end and esc on all tools
- [X] Pencil tool sample color with middle mouse button
- [X] Add sample tool merged layers property
- [X] Fix program failing to load some images which have the wrong extension for their format
- [X] Open image by passing it as a command line argument

## Alpha 0.5.0

- [X] BMP support
- [X] Add `credits` file
- [X] Fix too tall buttons in windows
- [X] Animation support (GIF, SCRBL)
  - [X] Add animation timeline GUI
  - [X] Basic timeline GUI functionality
  - [X] Animation settings window (frame time, loop)
  - [X] Add Animation and Frame classes
  - [X] Fully implement animation GUI
  - [X] Fix cropping to content race condition
  - [X] Add animation undo/redo
  - [X] Add .scrbl animation support
  - [X] Add 'Black Is Transparent' property to the animation settings
  - [X] Add .gif exporting
  - [X] Add .gif importing
  - [X] Add animation support in the backend
- [X] Add APNG support
  - [X] Add APNG exporting
  - [X] Add APNG importing
- [X] Fix crash with canvas rotation
- [X] Add default file extensions to the save and export dialogs
- [X] Fix bad performance when having many frames on large canvases
- [X] Fix Color selector not allowing keyboard input in the color value fields
- [X] Global "Replace Color" tool (replace all instances of a color with another)
- [X] Fix undoing and redoing layer creation changes the layer's id
- [X] Blend types (blend, overwrite) (Global setting (like brush size) selectable in tool properties)
- [X] Add credits to the about page

## UNREPRODUCIBLE BUGS

- [ ] Fix switching layer order crashing the program
- [X] **None known**

## Potential Features

- [ ] Symmetry tools (horizontal, vertical, diagonal, radial) (ie. mirror drawing mode)
- [ ] Gradient tool
- [ ] Reference image overlay
- [ ] Tabs for multiple images
- [ ] Text tool
- [ ] Plugin support
- [ ] Custom brushes
- [ ] File recovery
- [ ] Linux installer (Flatpak)
- [ ] Custom cursors
- [ ] Scale tool
