document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.football-field').forEach(field => {
        const formation = field.dataset.formation ?? "4-4-2";
        const playerSlots = field.querySelectorAll('.player-slot');

        // Pozisyona göre gruplar
        const defenders = ['CB', 'CB1', 'CB2', 'CB3', 'LB', 'RB', 'LWB', 'RWB'];
        const midfielders = ['CM', 'CM1', 'CM2', 'CM3', 'LM', 'RM', 'CAM', 'CDM'];
        const forwards = ['ST', 'ST1', 'ST2', 'LW', 'RW', 'CF'];

        // Pozisyona göre hangi gruba ait olduğunu dönen fonksiyon
        function getGroup(position) {
            if (defenders.some(d => position.startsWith(d))) return 'DEF';
            if (midfielders.some(m => position.startsWith(m))) return 'MID';
            if (forwards.some(f => position.startsWith(f))) return 'FWD';
            if (position === 'GK') return 'GK';
            return 'MID';
        }

        // Grupları baştan boş olarak oluşturuyoruz
        const groupedSlots = { GK: [], DEF: [], MID: [], FWD: [] };

        // Bütün oyuncuları ilgili gruba ekliyoruz
        playerSlots.forEach(p => {
            const group = getGroup(p.dataset.position);
            groupedSlots[group].push(p);
        });

        // Gruplara göre sahada dikeyde (Y ekseni) nerede duracaklarını belirliyoruz
        const positionsY = {
            GK: '85%',
            DEF: '63%',
            MID: '40%',
            FWD: '15%'
        };

        // Oyuncuları yatayda eşit aralıkla yerleştiriyoruz
        function distributeHorizontally(players, topPercent) {
            const count = players.length;
            players.forEach((p, i) => {
                const x = (100 / (count + 1)) * (i + 1);
                p.style.left = `${x}%`;
                p.style.top = topPercent;
            });
        }

        // Her grup için sahaya yerleştirme işlemini uyguluyoruz
        distributeHorizontally(groupedSlots.DEF, positionsY.DEF);
        distributeHorizontally(groupedSlots.MID, positionsY.MID);
        distributeHorizontally(groupedSlots.FWD, positionsY.FWD);
        distributeHorizontally(groupedSlots.GK, positionsY.GK);
    });
});
