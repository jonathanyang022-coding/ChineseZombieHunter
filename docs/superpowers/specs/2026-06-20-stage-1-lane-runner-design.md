# Stage 1 Lane Runner Design

**Goal:** Build a minimal portrait-oriented lane runner scene with a forward-facing camera, modular floor/walls, and basic left-right lane switching.

**Scope:** Stage 1 includes a single test scene, a simple player placeholder, a fixed forward camera, a repeated road segment layout, and a tiny obstacle placeholder set. It does not include shooting, health, scoring, enemy AI, or progression.

**Approach:** Reuse the Roll a Ball-style modular floor/wall construction pattern, but orient the scene so the camera looks down the road and the environment approaches the player. Keep the implementation primitive-based and script-light so it is easy to replace or expand later.

**Success Criteria:**
- The Game view is configured for a portrait 16:9 layout.
- The player stays centered near the bottom of the frame.
- The player can move between three lanes with basic input.
- The road, walls, and placeholder obstacles are readable from the forward-facing camera.

