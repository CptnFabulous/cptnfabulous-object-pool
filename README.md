To install, select the 'Add package from Git URL' option, then copy in 'https://github.com/CptnFabulous/cptnfabulous-object-pool.git'.
After adding the correct namespace to your script (CptnFabulous.ObjectPool), use ObjectPool.RequestObject() to spawn an item, and ObjectPool.DismissObject() to return it to the pool, the same way you'd use Object.Instantiate() and Object.Destroy().
