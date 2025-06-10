let maps = {};

window.leafletMaps = {
    initializeMap: function (mapId, latitude, longitude, title, zoom = 13) {
        try {
            // Check if map already exists and remove it
            if (maps[mapId]) {
                maps[mapId].remove();
                delete maps[mapId];
            }

            // Initialize the map
            const map = L.map(mapId).setView([latitude, longitude], zoom);

            // Add OpenStreetMap tile layer
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
                maxZoom: 19
            }).addTo(map);

            // Add marker
            const marker = L.marker([latitude, longitude])
                .addTo(map)
                .bindPopup(title)
                .openPopup();

            // Store map reference
            maps[mapId] = map;

            // Force map to resize after initialization
            setTimeout(() => {
                map.invalidateSize();
            }, 100);

            return true;
        } catch (error) {
            console.error('Error initializing map:', error);
            return false;
        }
    },

    resizeMap: function (mapId) {
        try {
            if (maps[mapId]) {
                maps[mapId].invalidateSize();
                return true;
            }
            return false;
        } catch (error) {
            console.error('Error resizing map:', error);
            return false;
        }
    },

    destroyMap: function (mapId) {
        try {
            if (maps[mapId]) {
                maps[mapId].remove();
                delete maps[mapId];
                return true;
            }
            return false;
        } catch (error) {
            console.error('Error destroying map:', error);
            return false;
        }
    },

    addMarker: function (mapId, latitude, longitude, title) {
        try {
            if (maps[mapId]) {
                const marker = L.marker([latitude, longitude])
                    .addTo(maps[mapId])
                    .bindPopup(title);
                return true;
            }
            return false;
        } catch (error) {
            console.error('Error adding marker:', error);
            return false;
        }
    },

    setView: function (mapId, latitude, longitude, zoom = 13) {
        try {
            if (maps[mapId]) {
                maps[mapId].setView([latitude, longitude], zoom);
                return true;
            }
            return false;
        } catch (error) {
            console.error('Error setting view:', error);
            return false;
        }
    }
}; 
