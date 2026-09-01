import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
  trailingSlash: 'always',
  integrations: [
    starlight({
      title: 'Archmage Docs',
      social: [
        { icon: 'github', label: 'GitHub', href: 'https://github.com/shadowopera/sdk-cs' }
      ],
      sidebar: [
        {
          label: 'C# SDK Overview',
          items: [{ autogenerate: { directory: 'overview-cs' } }],
        },
        {
          label: 'C# SDK',
          items: [{ autogenerate: { directory: 'sdk-cs' } }],
        },
        {
          label: 'C# SDK (Unity)',
          items: [{ autogenerate: { directory: 'sdk-cs-unity' } }],
        },
        {
          label: 'C# SDK (Unity Editor)',
          items: [{ autogenerate: { directory: 'sdk-cs-unity-editor' } }],
        },
        {
          label: 'C# Auto-Generated',
          items: [{ autogenerate: { directory: 'gen-cs' } }],
        },
      ],
    }),
  ],
});
