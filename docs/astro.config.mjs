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
          collapsed: true,
        },
        {
          label: 'C# SDK (Unity)',
          items: [{ autogenerate: { directory: 'sdk-cs-unity' } }],
          collapsed: true,
        },
        {
          label: 'C# SDK (Unity Editor)',
          items: [{ autogenerate: { directory: 'sdk-cs-unity-editor' } }],
          collapsed: true,
        },
        {
          label: 'C# Generated Code',
          items: [{ autogenerate: { directory: 'gen-cs' } }],
          collapsed: true,
        },
      ],
    }),
  ],
});
