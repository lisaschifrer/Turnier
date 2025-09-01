export interface Team {
  id: string;
  name: string;
  points: number;
  groupId?: string;
}

export interface PlacementParticipant {
  placementBracketId: string;
  teamId: string;
  seed: number;
  team: Team;
}

export interface PlacementBracket {
  id: string;
  turnierId: string;
  rankFromGroup: number; // 1..5
  placeMin: number;
  placeMax: number;
  name: string;
  participants: PlacementParticipant[];
}

export interface FinalMatch {
  id: string;
  placementBracketId: string;
  roundNumber: number;    // 1 für erste Runde
  indexInRound: number;   // 1..4
  teamAId: string;
  teamBId: string;
  winnerId?: string;

  // optional vom Backend expandiert:
  teamA?: Team;
  teamB?: Team;
}
