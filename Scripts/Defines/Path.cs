namespace Define
{
    public static class Path
    {
        public static class BGM
        {
            public const string BASE = "Sounds/BGMs/";

            public const string Title = "BGM-Title";
            public const string Battle = "BGM-Battle";
        }

        public static class Prefab
        {
            public const string BASE = "Prefabs/";

            public const string FloorButton = BASE + "WorldFloor";
        }

        public static class UI
        {
            public const string BASE = "Prefabs/UIs/";

            public const string FloorButton = "FloorButton";
            public const string HpSlider = "HpSlider";
            public const string FloatingDamageText = "FloatingDamageText";
            public const string AttackTarget = "AttackTarget";
            public const string AttackDirection = "AttackDirection";
        }

        public static class Models
        {
            public const string BASE = "Prefabs/Models/";
            public const string Unit = BASE + "Unit";
        }

        public static class Effects
        {
            public const string Base = "Effects/";

            public const string Explosion = Base + "Explosion";
            public const string Fireball = Base + "Fireball";
            public const string MeleeFire = Base + "MeleeFire";
        }

        /// <summary>
        /// Use only full path
        /// </summary>
        public static class Sprite
        {
            const string Base = "Sprites/";

            public const string Floor = Base + "Floor128";
            public const string FloorGrid = Base + "FloorGrid128";

            public const string WeaponBow = Base + "Weapons_Bow";
            public const string WeaponSword = Base + "Weapons_Sword";
            public const string WeaponAxe = Base + "Weapons_Axe";
        }
    }
}


