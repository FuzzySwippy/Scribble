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
- [X] Crop empty pixels in the layer visbility icon
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
- [ ] Fix history not registering drawing when started from a window that is being dismissed
- [ ] Tool shortcuts with tooltips
- [ ] Add About page