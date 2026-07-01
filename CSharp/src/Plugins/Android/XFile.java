// Copyright (c) 2026 FrameworkEase Technologies. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

package com.frameworkease.universal.x.utility;

import java.io.*;
import android.content.res.AssetFileDescriptor;

public class XFile {
    public static boolean HasAsset(String file){
        if (file.startsWith("jar:file://")) {
            String asset = file.substring(file.lastIndexOf("/") + 1);
            try {
                com.unity3d.player.UnityPlayer.currentActivity.getAssets().open(asset).close();
                return true;
            } catch (Exception e) {
                return false;
            }
        } else {
            return false;
        }
    }

    public static long AssetSize(String file) {
        if (file.startsWith("jar:file://")) {
            String asset = file.substring(file.lastIndexOf("/") + 1);
            try {
                AssetFileDescriptor fileDescriptor = com.unity3d.player.UnityPlayer.currentActivity.getAssets().openFd(asset);
                long size = fileDescriptor.getLength();
                fileDescriptor.close();
                return size;
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        return -1;
    }

    public static byte[] OpenAsset(String file) {
        if (file.startsWith("jar:file://")) {
            String asset = file.substring(file.lastIndexOf("/") + 1);
            try (InputStream is = com.unity3d.player.UnityPlayer.currentActivity.getAssets().open(asset);
                 ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                byte[] buffer = new byte[8192];
                int read;
                while ((read = is.read(buffer)) != -1) {
                    baos.write(buffer, 0, read);
                }
                return baos.toByteArray();
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        return null;
    }
}
