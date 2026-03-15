import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	integrations: [
		starlight({
			title: 'Archmage Documentation',
			social: [
				{ icon: 'github', label: 'GitHub', href: 'https://github.com/shadowopera/sdk-cs' }
			],
			sidebar: [
				{
					label: 'C# API Reference',
					autogenerate: { directory: 'sdk-cs' },
				},
			],
		}),
	],
});
