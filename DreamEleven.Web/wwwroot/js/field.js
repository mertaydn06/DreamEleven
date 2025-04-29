document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.football-field').forEach(field => {
        const formation = field.dataset.formation ?? "4-4-2";
        const playerSlots = field.querySelectorAll('.player-slot');

        // Pozisyona göre gruplar ve sıralama öncelikleri
        const positionPriorities = {
            // Defans öncelikleri (soldan sağa)
            'LWB': 0, 'LB': 1, 'CB1': 2, 'CB2': 3, 'CB3': 4, 'RB': 5, 'RWB': 6,
            // Orta saha öncelikleri (soldan sağa)
            'LM': 0, 'CM1': 1, 'CDM': 2, 'CM2': 3, 'CM3': 4, 'CAM': 5, 'RM': 6,
            // Forvet öncelikleri (soldan sağa)
            'LW': 0, 'ST1': 1, 'ST': 2, 'ST2': 3, 'CF': 4, 'RW': 5
        };

        const defenders = ['CB', 'CB1', 'CB2', 'CB3', 'LB', 'RB', 'LWB', 'RWB'];
        const midfielders = ['CM', 'CM1', 'CM2', 'CM3', 'LM', 'RM', 'CAM', 'CDM'];
        const forwards = ['ST', 'ST1', 'ST2', 'LW', 'RW', 'CF'];

        function getGroup(position) {
            if (defenders.some(d => position.startsWith(d))) return 'DEF';
            if (midfielders.some(m => position.startsWith(m))) return 'MID';
            if (forwards.some(f => position.startsWith(f))) return 'FWD';
            if (position === 'GK') return 'GK';
            return 'MID';
        }

        function getPositionPriority(position) {
            // Tam eşleşme varsa önceliği döndür
            if (position in positionPriorities) {
                return positionPriorities[position];
            }
            // Eşleşme yoksa, başlangıç karakterlerine göre kontrol et
            for (const [key, value] of Object.entries(positionPriorities)) {
                if (position.startsWith(key)) {
                    return value;
                }
            }
            return 999; // Bilinmeyen pozisyonlar için yüksek öncelik
        }

        const groupedSlots = { GK: [], DEF: [], MID: [], FWD: [] };

        // Oyuncuları gruplara eklerken pozisyon önceliklerine göre sırala
        const sortedSlots = Array.from(playerSlots).sort((a, b) => {
            const posA = a.dataset.position;
            const posB = b.dataset.position;
            return getPositionPriority(posA) - getPositionPriority(posB);
        });

        // Sıralanmış oyuncuları gruplara ekle
        sortedSlots.forEach(p => {
            const group = getGroup(p.dataset.position);
            groupedSlots[group].push(p);
        });

        const positionsY = {
            GK: '85%',
            DEF: '63%',
            MID: '40%',
            FWD: '15%'
        };

        function distributeHorizontally(players, topPercent) {
            const count = players.length;
            players.forEach((p, i) => {
                const x = (100 / (count + 1)) * (i + 1);
                p.style.left = `${x}%`;
                p.style.top = topPercent;
            });
        }

        distributeHorizontally(groupedSlots.DEF, positionsY.DEF);
        distributeHorizontally(groupedSlots.MID, positionsY.MID);
        distributeHorizontally(groupedSlots.FWD, positionsY.FWD);
        distributeHorizontally(groupedSlots.GK, positionsY.GK);
    });
});
