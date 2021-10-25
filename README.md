# TODO:
- Generalize Input for running
- Feedbacks (especially sounds)
- Fix Bugs

# Bugs:
- Moving platform has some problems (being inside of it is weird; it doesn't move the player along when going downwards; if not thin, it's worse) -> only for diagonally moving platforms ==> don't add them
- Wall grab sometimes doesn't proc when jumping from one wall to the opposite one
- Dashing against a wall prevents grabbing it
- Sometimes wall grabbing until you touch a thin platform will stop you, sometimes it won't
- sometimes you'll randomly pass through the ground
- up-down moving platforms move the player through the ground when under them
- too much landing particles