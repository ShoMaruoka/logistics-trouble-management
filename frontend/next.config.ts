import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: 'export',
  trailingSlash: true,
  basePath: process.env.NODE_ENV === 'production' ? '/ltm' : '/LTM',
  assetPrefix: process.env.NODE_ENV === 'production' ? '/ltm' : '/LTM',
  images: {
    unoptimized: true
  },
  // HTTPS対応
  experimental: {
    forceSwcTransforms: true
  },
  // 本番環境での設定
  env: {
    NEXT_PUBLIC_BASE_URL: process.env.NODE_ENV === 'production' 
      ? 'https://www.kaientai.cc' 
      : 'http://10.194.5.66:81',
    NEXT_PUBLIC_BASE_PATH: process.env.NODE_ENV === 'production' ? '/ltmapi' : '/LTMAPI',
    NEXT_PUBLIC_API_URL: process.env.NODE_ENV === 'production'
      ? 'https://www.kaientai.cc/ltmapi'
      : 'http://10.194.5.66:82/LTMAPI'
  }
};

export default nextConfig;
