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
          label: 'C# Guides',
          autogenerate: { directory: 'guides-cs' },
        },
        {
          label: 'C# Auto-Generated',
          autogenerate: { directory: 'gen-cs' },
        },
        {
          label: 'C# SDK',
          autogenerate: { directory: 'sdk-cs' },
        },
        {
          label: 'C# SDK (Unity)',
          autogenerate: { directory: 'sdk-cs-unity' },
        },
        {
          label: 'C# SDK (Unity Editor)',
          autogenerate: { directory: 'sdk-cs-unity-editor' },
        },
      ],
    }),
  ],
});
